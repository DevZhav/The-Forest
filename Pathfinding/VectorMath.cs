﻿using System;
using UnityEngine;

namespace Pathfinding
{
	
	public static class VectorMath
	{
		
		public static Vector3 ClosestPointOnLine(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
		{
			Vector3 vector = Vector3.Normalize(lineEnd - lineStart);
			float d = Vector3.Dot(point - lineStart, vector);
			return lineStart + d * vector;
		}

		
		public static float ClosestPointOnLineFactor(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
		{
			Vector3 rhs = lineEnd - lineStart;
			float sqrMagnitude = rhs.sqrMagnitude;
			if ((double)sqrMagnitude <= 1E-06)
			{
				return 0f;
			}
			return Vector3.Dot(point - lineStart, rhs) / sqrMagnitude;
		}

		
		public static float ClosestPointOnLineFactor(Int3 lineStart, Int3 lineEnd, Int3 point)
		{
			Int3 rhs = lineEnd - lineStart;
			float sqrMagnitude = rhs.sqrMagnitude;
			float num = (float)Int3.Dot(point - lineStart, rhs);
			if (sqrMagnitude != 0f)
			{
				num /= sqrMagnitude;
			}
			return num;
		}

		
		public static float ClosestPointOnLineFactor(Int2 lineStart, Int2 lineEnd, Int2 point)
		{
			Int2 b = lineEnd - lineStart;
			double num = (double)b.sqrMagnitudeLong;
			double num2 = (double)Int2.DotLong(point - lineStart, b);
			if (num != 0.0)
			{
				num2 /= num;
			}
			return (float)num2;
		}

		
		public static Vector3 ClosestPointOnSegment(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
		{
			Vector3 vector = lineEnd - lineStart;
			float sqrMagnitude = vector.sqrMagnitude;
			if ((double)sqrMagnitude <= 1E-06)
			{
				return lineStart;
			}
			float value = Vector3.Dot(point - lineStart, vector) / sqrMagnitude;
			return lineStart + Mathf.Clamp01(value) * vector;
		}

		
		public static Vector3 ClosestPointOnSegmentXZ(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
		{
			lineStart.y = point.y;
			lineEnd.y = point.y;
			Vector3 vector = lineEnd - lineStart;
			Vector3 a = vector;
			a.y = 0f;
			float magnitude = a.magnitude;
			Vector3 vector2 = (magnitude <= float.Epsilon) ? Vector3.zero : (a / magnitude);
			float value = Vector3.Dot(point - lineStart, vector2);
			return lineStart + Mathf.Clamp(value, 0f, a.magnitude) * vector2;
		}

		
		public static float SqrDistancePointSegmentApproximate(int x, int z, int px, int pz, int qx, int qz)
		{
			float num = (float)(qx - px);
			float num2 = (float)(qz - pz);
			float num3 = (float)(x - px);
			float num4 = (float)(z - pz);
			float num5 = num * num + num2 * num2;
			float num6 = num * num3 + num2 * num4;
			if (num5 > 0f)
			{
				num6 /= num5;
			}
			if (num6 < 0f)
			{
				num6 = 0f;
			}
			else if (num6 > 1f)
			{
				num6 = 1f;
			}
			num3 = (float)px + num6 * num - (float)x;
			num4 = (float)pz + num6 * num2 - (float)z;
			return num3 * num3 + num4 * num4;
		}

		
		public static float SqrDistancePointSegmentApproximate(Int3 a, Int3 b, Int3 p)
		{
			float num = (float)(b.x - a.x);
			float num2 = (float)(b.z - a.z);
			float num3 = (float)(p.x - a.x);
			float num4 = (float)(p.z - a.z);
			float num5 = num * num + num2 * num2;
			float num6 = num * num3 + num2 * num4;
			if (num5 > 0f)
			{
				num6 /= num5;
			}
			if (num6 < 0f)
			{
				num6 = 0f;
			}
			else if (num6 > 1f)
			{
				num6 = 1f;
			}
			num3 = (float)a.x + num6 * num - (float)p.x;
			num4 = (float)a.z + num6 * num2 - (float)p.z;
			return num3 * num3 + num4 * num4;
		}

		
		public static float SqrDistancePointSegment(Vector3 a, Vector3 b, Vector3 p)
		{
			Vector3 a2 = VectorMath.ClosestPointOnSegment(a, b, p);
			return (a2 - p).sqrMagnitude;
		}

		
		public static float SqrDistanceSegmentSegment(Vector3 s1, Vector3 e1, Vector3 s2, Vector3 e2)
		{
			Vector3 vector = e1 - s1;
			Vector3 vector2 = e2 - s2;
			Vector3 vector3 = s1 - s2;
			float num = Vector3.Dot(vector, vector);
			float num2 = Vector3.Dot(vector, vector2);
			float num3 = Vector3.Dot(vector2, vector2);
			float num4 = Vector3.Dot(vector, vector3);
			float num5 = Vector3.Dot(vector2, vector3);
			float num6 = num * num3 - num2 * num2;
			float num7 = num6;
			float num8 = num6;
			float num9;
			float num10;
			if (num6 < 1E-06f)
			{
				num9 = 0f;
				num7 = 1f;
				num10 = num5;
				num8 = num3;
			}
			else
			{
				num9 = num2 * num5 - num3 * num4;
				num10 = num * num5 - num2 * num4;
				if (num9 < 0f)
				{
					num9 = 0f;
					num10 = num5;
					num8 = num3;
				}
				else if (num9 > num7)
				{
					num9 = num7;
					num10 = num5 + num2;
					num8 = num3;
				}
			}
			if (num10 < 0f)
			{
				num10 = 0f;
				if (-num4 < 0f)
				{
					num9 = 0f;
				}
				else if (-num4 > num)
				{
					num9 = num7;
				}
				else
				{
					num9 = -num4;
					num7 = num;
				}
			}
			else if (num10 > num8)
			{
				num10 = num8;
				if (-num4 + num2 < 0f)
				{
					num9 = 0f;
				}
				else if (-num4 + num2 > num)
				{
					num9 = num7;
				}
				else
				{
					num9 = -num4 + num2;
					num7 = num;
				}
			}
			float d = (Math.Abs(num9) >= 1E-06f) ? (num9 / num7) : 0f;
			float d2 = (Math.Abs(num10) >= 1E-06f) ? (num10 / num8) : 0f;
			return (vector3 + d * vector - d2 * vector2).sqrMagnitude;
		}

		
		public static float SqrDistanceXZ(Vector3 a, Vector3 b)
		{
			Vector3 vector = a - b;
			return vector.x * vector.x + vector.z * vector.z;
		}

		
		public static long SignedTriangleAreaTimes2XZ(Int3 a, Int3 b, Int3 c)
		{
			return (long)(b.x - a.x) * (long)(c.z - a.z) - (long)(c.x - a.x) * (long)(b.z - a.z);
		}

		
		public static float SignedTriangleAreaTimes2XZ(Vector3 a, Vector3 b, Vector3 c)
		{
			return (b.x - a.x) * (c.z - a.z) - (c.x - a.x) * (b.z - a.z);
		}

		
		public static bool RightXZ(Vector3 a, Vector3 b, Vector3 p)
		{
			return (b.x - a.x) * (p.z - a.z) - (p.x - a.x) * (b.z - a.z) < -1.401298E-45f;
		}

		
		public static bool RightXZ(Int3 a, Int3 b, Int3 p)
		{
			return (long)(b.x - a.x) * (long)(p.z - a.z) - (long)(p.x - a.x) * (long)(b.z - a.z) < 0L;
		}

		
		public static bool RightOrColinear(Vector2 a, Vector2 b, Vector2 p)
		{
			return (b.x - a.x) * (p.y - a.y) - (p.x - a.x) * (b.y - a.y) <= 0f;
		}

		
		public static bool RightOrColinear(Int2 a, Int2 b, Int2 p)
		{
			return (long)(b.x - a.x) * (long)(p.y - a.y) - (long)(p.x - a.x) * (long)(b.y - a.y) <= 0L;
		}

		
		public static bool RightOrColinearXZ(Vector3 a, Vector3 b, Vector3 p)
		{
			return (b.x - a.x) * (p.z - a.z) - (p.x - a.x) * (b.z - a.z) <= 0f;
		}

		
		public static bool RightOrColinearXZ(Int3 a, Int3 b, Int3 p)
		{
			return (long)(b.x - a.x) * (long)(p.z - a.z) - (long)(p.x - a.x) * (long)(b.z - a.z) <= 0L;
		}

		
		public static bool IsClockwiseMarginXZ(Vector3 a, Vector3 b, Vector3 c)
		{
			return (b.x - a.x) * (c.z - a.z) - (c.x - a.x) * (b.z - a.z) <= float.Epsilon;
		}

		
		public static bool IsClockwiseXZ(Vector3 a, Vector3 b, Vector3 c)
		{
			return (b.x - a.x) * (c.z - a.z) - (c.x - a.x) * (b.z - a.z) < 0f;
		}

		
		public static bool IsClockwiseXZ(Int3 a, Int3 b, Int3 c)
		{
			return VectorMath.RightXZ(a, b, c);
		}

		
		public static bool IsClockwiseOrColinearXZ(Int3 a, Int3 b, Int3 c)
		{
			return VectorMath.RightOrColinearXZ(a, b, c);
		}

		
		public static bool IsClockwiseOrColinear(Int2 a, Int2 b, Int2 c)
		{
			return VectorMath.RightOrColinear(a, b, c);
		}

		
		public static bool IsColinearXZ(Int3 a, Int3 b, Int3 c)
		{
			return (long)(b.x - a.x) * (long)(c.z - a.z) - (long)(c.x - a.x) * (long)(b.z - a.z) == 0L;
		}

		
		public static bool IsColinearXZ(Vector3 a, Vector3 b, Vector3 c)
		{
			float num = (b.x - a.x) * (c.z - a.z) - (c.x - a.x) * (b.z - a.z);
			return num <= 1E-07f && num >= -1E-07f;
		}

		
		public static bool IsColinearAlmostXZ(Int3 a, Int3 b, Int3 c)
		{
			long num = (long)(b.x - a.x) * (long)(c.z - a.z) - (long)(c.x - a.x) * (long)(b.z - a.z);
			return num > -1L && num < 1L;
		}

		
		public static bool SegmentsIntersect(Int2 start1, Int2 end1, Int2 start2, Int2 end2)
		{
			return VectorMath.RightOrColinear(start1, end1, start2) != VectorMath.RightOrColinear(start1, end1, end2) && VectorMath.RightOrColinear(start2, end2, start1) != VectorMath.RightOrColinear(start2, end2, end1);
		}

		
		public static bool SegmentsIntersectXZ(Int3 start1, Int3 end1, Int3 start2, Int3 end2)
		{
			return VectorMath.RightOrColinearXZ(start1, end1, start2) != VectorMath.RightOrColinearXZ(start1, end1, end2) && VectorMath.RightOrColinearXZ(start2, end2, start1) != VectorMath.RightOrColinearXZ(start2, end2, end1);
		}

		
		public static bool SegmentsIntersectXZ(Vector3 start1, Vector3 end1, Vector3 start2, Vector3 end2)
		{
			Vector3 vector = end1 - start1;
			Vector3 vector2 = end2 - start2;
			float num = vector2.z * vector.x - vector2.x * vector.z;
			if (num == 0f)
			{
				return false;
			}
			float num2 = vector2.x * (start1.z - start2.z) - vector2.z * (start1.x - start2.x);
			float num3 = vector.x * (start1.z - start2.z) - vector.z * (start1.x - start2.x);
			float num4 = num2 / num;
			float num5 = num3 / num;
			return num4 >= 0f && num4 <= 1f && num5 >= 0f && num5 <= 1f;
		}

		
		public static Vector3 LineDirIntersectionPointXZ(Vector3 start1, Vector3 dir1, Vector3 start2, Vector3 dir2)
		{
			float num = dir2.z * dir1.x - dir2.x * dir1.z;
			if (num == 0f)
			{
				return start1;
			}
			float num2 = dir2.x * (start1.z - start2.z) - dir2.z * (start1.x - start2.x);
			float d = num2 / num;
			return start1 + dir1 * d;
		}

		
		public static Vector3 LineDirIntersectionPointXZ(Vector3 start1, Vector3 dir1, Vector3 start2, Vector3 dir2, out bool intersects)
		{
			float num = dir2.z * dir1.x - dir2.x * dir1.z;
			if (num == 0f)
			{
				intersects = false;
				return start1;
			}
			float num2 = dir2.x * (start1.z - start2.z) - dir2.z * (start1.x - start2.x);
			float d = num2 / num;
			intersects = true;
			return start1 + dir1 * d;
		}

		
		public static bool RaySegmentIntersectXZ(Int3 start1, Int3 end1, Int3 start2, Int3 end2)
		{
			Int3 @int = end1 - start1;
			Int3 int2 = end2 - start2;
			long num = (long)(int2.z * @int.x - int2.x * @int.z);
			if (num == 0L)
			{
				return false;
			}
			long num2 = (long)(int2.x * (start1.z - start2.z) - int2.z * (start1.x - start2.x));
			long num3 = (long)(@int.x * (start1.z - start2.z) - @int.z * (start1.x - start2.x));
			return (num2 < 0L ^ num < 0L) && (num3 < 0L ^ num < 0L) && (num < 0L || num3 <= num) && (num >= 0L || num3 > num);
		}

		
		public static bool LineIntersectionFactorXZ(Int3 start1, Int3 end1, Int3 start2, Int3 end2, out float factor1, out float factor2)
		{
			Int3 @int = end1 - start1;
			Int3 int2 = end2 - start2;
			long num = (long)(int2.z * @int.x - int2.x * @int.z);
			if (num == 0L)
			{
				factor1 = 0f;
				factor2 = 0f;
				return false;
			}
			long num2 = (long)(int2.x * (start1.z - start2.z) - int2.z * (start1.x - start2.x));
			long num3 = (long)(@int.x * (start1.z - start2.z) - @int.z * (start1.x - start2.x));
			factor1 = (float)num2 / (float)num;
			factor2 = (float)num3 / (float)num;
			return true;
		}

		
		public static bool LineIntersectionFactorXZ(Vector3 start1, Vector3 end1, Vector3 start2, Vector3 end2, out float factor1, out float factor2)
		{
			Vector3 vector = end1 - start1;
			Vector3 vector2 = end2 - start2;
			float num = vector2.z * vector.x - vector2.x * vector.z;
			if (num <= 1E-05f && num >= -1E-05f)
			{
				factor1 = 0f;
				factor2 = 0f;
				return false;
			}
			float num2 = vector2.x * (start1.z - start2.z) - vector2.z * (start1.x - start2.x);
			float num3 = vector.x * (start1.z - start2.z) - vector.z * (start1.x - start2.x);
			float num4 = num2 / num;
			float num5 = num3 / num;
			factor1 = num4;
			factor2 = num5;
			return true;
		}

		
		public static float LineRayIntersectionFactorXZ(Int3 start1, Int3 end1, Int3 start2, Int3 end2)
		{
			Int3 @int = end1 - start1;
			Int3 int2 = end2 - start2;
			int num = int2.z * @int.x - int2.x * @int.z;
			if (num == 0)
			{
				return float.NaN;
			}
			int num2 = int2.x * (start1.z - start2.z) - int2.z * (start1.x - start2.x);
			int num3 = @int.x * (start1.z - start2.z) - @int.z * (start1.x - start2.x);
			if ((float)num3 / (float)num < 0f)
			{
				return float.NaN;
			}
			return (float)num2 / (float)num;
		}

		
		public static float LineIntersectionFactorXZ(Vector3 start1, Vector3 end1, Vector3 start2, Vector3 end2)
		{
			Vector3 vector = end1 - start1;
			Vector3 vector2 = end2 - start2;
			float num = vector2.z * vector.x - vector2.x * vector.z;
			if (num == 0f)
			{
				return -1f;
			}
			float num2 = vector2.x * (start1.z - start2.z) - vector2.z * (start1.x - start2.x);
			return num2 / num;
		}

		
		public static Vector3 LineIntersectionPointXZ(Vector3 start1, Vector3 end1, Vector3 start2, Vector3 end2)
		{
			bool flag;
			return VectorMath.LineIntersectionPointXZ(start1, end1, start2, end2, out flag);
		}

		
		public static Vector3 LineIntersectionPointXZ(Vector3 start1, Vector3 end1, Vector3 start2, Vector3 end2, out bool intersects)
		{
			Vector3 a = end1 - start1;
			Vector3 vector = end2 - start2;
			float num = vector.z * a.x - vector.x * a.z;
			if (num == 0f)
			{
				intersects = false;
				return start1;
			}
			float num2 = vector.x * (start1.z - start2.z) - vector.z * (start1.x - start2.x);
			float d = num2 / num;
			intersects = true;
			return start1 + a * d;
		}

		
		public static Vector2 LineIntersectionPoint(Vector2 start1, Vector2 end1, Vector2 start2, Vector2 end2)
		{
			bool flag;
			return VectorMath.LineIntersectionPoint(start1, end1, start2, end2, out flag);
		}

		
		public static Vector2 LineIntersectionPoint(Vector2 start1, Vector2 end1, Vector2 start2, Vector2 end2, out bool intersects)
		{
			Vector2 a = end1 - start1;
			Vector2 vector = end2 - start2;
			float num = vector.y * a.x - vector.x * a.y;
			if (num == 0f)
			{
				intersects = false;
				return start1;
			}
			float num2 = vector.x * (start1.y - start2.y) - vector.y * (start1.x - start2.x);
			float d = num2 / num;
			intersects = true;
			return start1 + a * d;
		}

		
		public static Vector3 SegmentIntersectionPointXZ(Vector3 start1, Vector3 end1, Vector3 start2, Vector3 end2, out bool intersects)
		{
			Vector3 a = end1 - start1;
			Vector3 vector = end2 - start2;
			float num = vector.z * a.x - vector.x * a.z;
			if (num == 0f)
			{
				intersects = false;
				return start1;
			}
			float num2 = vector.x * (start1.z - start2.z) - vector.z * (start1.x - start2.x);
			float num3 = a.x * (start1.z - start2.z) - a.z * (start1.x - start2.x);
			float num4 = num2 / num;
			float num5 = num3 / num;
			if (num4 < 0f || num4 > 1f || num5 < 0f || num5 > 1f)
			{
				intersects = false;
				return start1;
			}
			intersects = true;
			return start1 + a * num4;
		}

		
		public static bool SegmentIntersectsBounds(Bounds bounds, Vector3 a, Vector3 b)
		{
			a -= bounds.center;
			b -= bounds.center;
			Vector3 b2 = (a + b) * 0.5f;
			Vector3 vector = a - b2;
			Vector3 vector2 = new Vector3(Math.Abs(vector.x), Math.Abs(vector.y), Math.Abs(vector.z));
			Vector3 extents = bounds.extents;
			return Math.Abs(b2.x) <= extents.x + vector2.x && Math.Abs(b2.y) <= extents.y + vector2.y && Math.Abs(b2.z) <= extents.z + vector2.z && Math.Abs(b2.y * vector.z - b2.z * vector.y) <= extents.y * vector2.z + extents.z * vector2.y && Math.Abs(b2.x * vector.z - b2.z * vector.x) <= extents.x * vector2.z + extents.z * vector2.x && Math.Abs(b2.x * vector.y - b2.y * vector.x) <= extents.x * vector2.y + extents.y * vector2.x;
		}

		
		public static float LineCircleIntersectionFactor(Vector3 circleCenter, Vector3 linePoint1, Vector3 linePoint2, float radius)
		{
			float num;
			Vector3 rhs = VectorMath.Normalize(linePoint2 - linePoint1, out num);
			Vector3 lhs = linePoint1 - circleCenter;
			float num2 = Vector3.Dot(lhs, rhs);
			float num3 = num2 * num2 - (lhs.sqrMagnitude - radius * radius);
			if (num3 < 0f)
			{
				num3 = 0f;
			}
			float num4 = -num2 + Mathf.Sqrt(num3);
			return (num <= 1E-05f) ? 0f : (num4 / num);
		}

		
		public static bool ReversesFaceOrientations(Matrix4x4 matrix)
		{
			Vector3 lhs = matrix.MultiplyVector(new Vector3(1f, 0f, 0f));
			Vector3 rhs = matrix.MultiplyVector(new Vector3(0f, 1f, 0f));
			Vector3 rhs2 = matrix.MultiplyVector(new Vector3(0f, 0f, 1f));
			float num = Vector3.Dot(Vector3.Cross(lhs, rhs), rhs2);
			return num < 0f;
		}

		
		public static bool ReversesFaceOrientationsXZ(Matrix4x4 matrix)
		{
			Vector3 vector = matrix.MultiplyVector(new Vector3(1f, 0f, 0f));
			Vector3 vector2 = matrix.MultiplyVector(new Vector3(0f, 0f, 1f));
			float num = vector.x * vector2.z - vector2.x * vector.z;
			return num < 0f;
		}

		
		public static Vector3 Normalize(Vector3 v, out float magnitude)
		{
			magnitude = v.magnitude;
			if (magnitude > 1E-05f)
			{
				return v / magnitude;
			}
			return Vector3.zero;
		}

		
		public static Vector2 Normalize(Vector2 v, out float magnitude)
		{
			magnitude = v.magnitude;
			if (magnitude > 1E-05f)
			{
				return v / magnitude;
			}
			return Vector2.zero;
		}

		
		public static Vector3 ClampMagnitudeXZ(Vector3 v, float maxMagnitude)
		{
			float num = v.x * v.x + v.z * v.z;
			if (num > maxMagnitude * maxMagnitude && maxMagnitude > 0f)
			{
				float num2 = maxMagnitude / Mathf.Sqrt(num);
				v.x *= num2;
				v.z *= num2;
			}
			return v;
		}

		
		public static float MagnitudeXZ(Vector3 v)
		{
			return Mathf.Sqrt(v.x * v.x + v.z * v.z);
		}
	}
}
