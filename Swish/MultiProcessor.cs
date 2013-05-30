using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace Swish
{
	public class MultiProcessor: IDisposable
	{
		public delegate List<string> WorkFunction(string data);

		public class Job
		{
			public WorkFunction Function { get; set; }
			public string Data { get; set; }
			public Exception Error { get; set; }
			public List<string> Result { get; set; }
			public int ID { get; set; }
		}

		public class JobList
		{
			private class Link
			{
				internal Link Next { get; set; }
				internal Job Job { get; set; }
			}

			internal JobList()
			{
				_end = new Link();
				_start = _end;
			}

			private Link _end;
			private Link _start;

			internal bool MoreItems
			{
				get { return _start.Job != null; }
			}

			int _addedJobs;
			int _removedJobs;

			internal void Add(Job job)
			{
				Link link = new Link();
				_end.Next = link;
				_end.Job = job;
				_end = link;
				_addedJobs++;
			}
			public Job Remove()
			{
				Job job = _start.Job;
				if (job == null)
				{
					throw new Exception();
				}
				_start = _start.Next;
				_removedJobs++;

				return job;
			}

			internal int Count
			{
				get { return _addedJobs - _removedJobs; }
			}
		}

		private class ThreadState: IDisposable
		{
			internal ThreadState()
			{
				_thread = new Thread(ThreadMain);
				_thread.IsBackground = true;
				_thread.Name = "ThreadedProcessor worker thread";
				_thread.Start();
			}

			private Thread _thread;
			private bool _stop;

			private JobList _pendingJobs = new JobList();
			internal JobList PendingJobs { get { return _pendingJobs; } }

			private JobList _finishedJobs = new JobList();
			internal JobList FinishedJobs { get { return _finishedJobs; } }

			internal bool Active { get; set; }

			internal bool Closed { get; set; }

			internal void Close()
			{
				_stop = true;
				while (!Closed)
				{
					Thread.Sleep(1);
				}
				_thread = null;
			}

			private void ThreadMain(object stateObject)
			{
				try
				{
					while (true)
					{
						if (_stop)
						{
							Active = false;
							break;
						}
						if (!_pendingJobs.MoreItems)
						{
							Active = false;
							if (SpinWait)
							{
								Thread.Yield();
							} else
							{
								Thread.Sleep(1);
							}
							continue;
						}

						Active = true;

						Job job = _pendingJobs.Remove();
						try
						{
							job.Result = job.Function(job.Data);
						} catch (Exception error)
						{
							job.Error = error;
						}

						_finishedJobs.Add(job);
					}
				} finally
				{
					Closed = true;
					_thread = null;
				}
			}

			void IDisposable.Dispose()
			{
				Close();
			}

			public bool SpinWait { get; set; }
		}

		public event ProgressChangedEventHandler ProgressChanged;
		public event RunWorkerCompletedEventHandler RunCompleted;

		public bool SingleThreaded { get; set; }
		public bool SpinWait { get; set; }
		public int QueueSize { get; set; }

		private int _addedCount;
		private int _progress;

		private JobList _pendingList = new JobList();
		private JobList _finishedJobs = new JobList();
		public JobList FinishedJobs { get { return _finishedJobs; } }
		private JobList _failedJobs = new JobList();
		public JobList FailedJobs { get { return _failedJobs; } }

		private BackgroundWorker _masterThread;

		public bool Finished { get { return _masterThread == null; } }

		public MultiProcessor()
		{
			SingleThreaded = Environment.ProcessorCount == 1;
			QueueSize = 2;
		}

		private void Add(WorkFunction Function, string data, int ID)
		{
			if (Function == null)
			{
				throw new ArgumentNullException("FunctionS");
			}
			Job job = new Job();
			job.Function = Function;
			job.Data = data;
			job.ID = ID;
			_pendingList.Add(job);
			_addedCount++;
		}

		public void Add(WorkFunction Function, string data)
		{
			Add(Function, data, -1);
		}

		public void Run()
		{
			if (_masterThread != null)
			{
				throw new Exception();
			}
			_masterThread = new BackgroundWorker();
			_masterThread.WorkerReportsProgress = true;
			_masterThread.WorkerSupportsCancellation = true;
			_masterThread.DoWork += new DoWorkEventHandler(MasterThreadMain);
			_masterThread.RunWorkerAsync();
		}

		public void WaitUntilFinished()
		{
			while (_masterThread != null)
			{
				Thread.Sleep(1);
			}
		}

		public void Cancel()
		{
			if (_masterThread != null)
			{
				_masterThread.CancelAsync();
			}
		}

		private void MasterThreadMain(object sender, DoWorkEventArgs e)
		{
			try
			{
				_progress = -1;
				UpdateProgress(0);

				if (!SingleThreaded)
				{
					int threadCount;
					if (!SpinWait)
					{
						threadCount = Environment.ProcessorCount;
					} else
					{
						threadCount = Environment.ProcessorCount;
						threadCount = Math.Max(1, threadCount - 2);
					}
					List<ThreadState> _workerThreads = new List<ThreadState>();
					while (_workerThreads.Count < threadCount)
					{
						ThreadState state = new ThreadState();
						state.SpinWait = SpinWait;
						_workerThreads.Add(state);
					}

					int threadIndex = 0;
					int startedCount = 0;
					int finishedCount = 0;
					while (true)
					{
						if (_masterThread.CancellationPending)
						{
							break;
						}
						bool jobAdded = false;
						while (_pendingList.MoreItems)
						{
							int nextThreadIndex = -1;
							if (!SpinWait)
							{
								int nextThreadCount = -1;
								for (int index = 0; index < _workerThreads.Count; index++)
								{
									int testIndex = (threadIndex + index) % _workerThreads.Count;

									ThreadState state = _workerThreads[testIndex];
									int jobCount = state.PendingJobs.Count;
									if (jobCount < nextThreadCount || (jobCount < QueueSize && nextThreadIndex == -1))
									{
										nextThreadCount = jobCount;
										nextThreadIndex = testIndex;
										if (jobCount == 0)
										{
											break;
										}
									}
								}
							} else
							{
								for (int index = 0; index < _workerThreads.Count; index++)
								{
									int testIndex = (threadIndex + index) % _workerThreads.Count;

									ThreadState state = _workerThreads[testIndex];
									int jobCount = state.PendingJobs.Count;
									if (jobCount < QueueSize)
									{
										nextThreadIndex = testIndex;
										break;
									}
								}
							}

							if (nextThreadIndex == -1)
							{
								break;
							}

							Job job = _pendingList.Remove();
							_workerThreads[nextThreadIndex].PendingJobs.Add(job);
							threadIndex = (nextThreadIndex + 1) % _workerThreads.Count;
							startedCount++;
							jobAdded = true;
						}

						bool threadsActive = false;
						bool jobFinished = false;
						for (int index = 0; index < _workerThreads.Count; index++)
						{
							ThreadState state = _workerThreads[(threadIndex + index) % _workerThreads.Count];

							while (state.FinishedJobs.MoreItems)
							{
								Job job = state.FinishedJobs.Remove();
								finishedCount++;
								if (job.Error != null)
								{
									_failedJobs.Add(job);
								} else
								{
									_finishedJobs.Add(job);
								}

								UpdateProgress(finishedCount);
								jobFinished = true;
							}

							if (state.PendingJobs.MoreItems || state.Active || state.FinishedJobs.MoreItems)
							{
								threadsActive = true;
							}
						}
						if (!_pendingList.MoreItems && !threadsActive)
						{
							break;
						}
						if (!jobAdded && !jobFinished)
						{
							if (SpinWait)
							{
								Thread.Yield();
							} else
							{
								Thread.Sleep(1);
							}
						}
					}

					// close the threads
					for (int index = 0; index < _workerThreads.Count; index++)
					{
						ThreadState state = _workerThreads[index];
						state.Close();
						using (state) { }
					}
				} else
				{
					int index = 0;
					while (_pendingList.MoreItems)
					{
						if (_masterThread.CancellationPending)
						{
							break;
						}
						Job job = _pendingList.Remove();

						try
						{
							job.Result = job.Function(job.Data);
							_finishedJobs.Add(job);
						} catch (Exception error)
						{
							job.Error = error;
							_failedJobs.Add(job);
						}

						UpdateProgress(index + 1);
						index++;
					}
				}

				if (RunCompleted != null)
				{
					try
					{
						RunCompleted(this, new RunWorkerCompletedEventArgs(_failedJobs, null, _masterThread.CancellationPending));
					} catch { }
				}
			} finally
			{
				_masterThread = null;
			}
		}

		private void UpdateProgress(int finishedCount)
		{
			int currentProgress;
			if (_addedCount > 0)
			{
				currentProgress = finishedCount * 100 / _addedCount;
			} else
			{
				currentProgress = 0;
			}
			if (currentProgress != _progress)
			{
				if (ProgressChanged != null)
				{
					try
					{
						ProgressChanged(this, new ProgressChangedEventArgs(currentProgress, null));
					} catch { }
				}
				_progress = currentProgress;
			}
		}

		void IDisposable.Dispose()
		{
			Cancel();
			WaitUntilFinished();
			using (_masterThread) { }
		}

		public static List<List<string>> ProcessList(IList<string> values, WorkFunction Function)
		{
			List<List<string>> results;
			using (MultiProcessor processor = new MultiProcessor())
			{
				for (int valueIndex = 0; valueIndex < values.Count; valueIndex++)
				{
					string data = values[valueIndex];
					int ID = valueIndex;
					processor.Add(Function, data, ID);
				}
				processor.SpinWait = true;
				processor.QueueSize = 10;
				processor.Run();
				processor.WaitUntilFinished();

				List<string>[] resultsArray = new List<string>[values.Count];

				if (processor.FailedJobs.MoreItems)
				{
					throw new Exception();
				}
				int finishedCount = 0;
				while (processor.FinishedJobs.MoreItems)
				{
					Job job = processor.FinishedJobs.Remove();
					int resultIndex = job.ID;
					resultsArray[resultIndex] = job.Result;
					finishedCount++;
				}
				if (finishedCount != values.Count)
				{
					throw new Exception();
				}
				results = new List<List<string>>(resultsArray);
			}
			return results;
		}




	}
}

