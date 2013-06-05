
namespace Swish
{
	public enum MergeRecordResult
	{
		Unknown = 0,
		///  observation appeared in master only
		Master =1,
		///  observation appeared in using only
		Using = 2,
		///  observation appeared in both
		Match = 4,
		///  observation appeared in both, missing values updated
		MatchUpdate = 8,
		///  observation appeared in both, conflicting nonmissing values
		MatchConflict = 16,
	}
}
