/*
* Copyright 2022-2023 Sony Corporation
*/
using Mocopi.Mobile.Sdk.Common;
using System;

namespace Mocopi.Mobile.Sdk
{
    /// <summary>
    /// @~japanese 身長設定用構造体@n
    /// @~
    /// </summary>
    public struct MocopiHeightStruct
    {
		/// <summary>
		/// @~japanese 1フィートあたりのメートル値@n
		/// @~
		/// </summary>
		public const float ONE_FEET = 0.3048f;

		/// <summary>
		/// @~japanese 1インチあたりのメートル値@n
		/// @~
		/// </summary>
		public const float ONE_INCH = 0.0254f;
		
        /// <summary>
        /// @~japanese 身長(meter)@n
        /// @~
        /// </summary>
        public float Meter { get; set; }

        /// <summary>
        /// @~japanese 身長(Feet)@n
        /// @~
        /// </summary>
        /// <remarks>
        /// @~japanese @ref Inch と1セットで使用する。@n
        /// @~
        /// </remarks>
        public int Feet { get; set; }

        /// <summary>
        /// @~japanese 身長(Inch)@n
        /// @~
        /// </summary>
        /// <remarks>
        /// @~japanese @ref Feet と1セットで使用する。@n
        /// @~
        /// </remarks>
        public int Inch { get; set; }

        /// <summary>
        /// @~japanese 身長の単位@n
        /// @~
        /// </summary>
        public EnumHeightUnit Unit { get; set; }

        /// <summary>
        /// @~japanese フィート/インチからメートルへ変換する。@n
        /// @~
        /// </summary>
        /// <param name="feet">
		/// @~japanese フィート@n
		/// @~
		/// </param>
        /// <param name="inch">
		/// @~japanese インチ@n
		/// @~
		/// </param>
        /// <returns>
		/// @~japanese メートル(m)@n
		/// @~
		/// </returns>
        public float ConvertFeetAndInchIntoMeter(int feet, int inch)
        {
            float height = feet * ONE_FEET + inch * ONE_INCH;
            return (float)Math.Round(height, 2);
        }

        /// <summary>
        /// @~japanese メートルからフィート/インチへ変換する。@n
        /// @~
        /// </summary>
        /// <param name="meter">
		/// @~japanese メートル@n
		/// @~
		/// </param>
        /// <returns>
		/// @~japanese フィート/インチ@n
		/// @~
		/// </returns>
		/// <returns>
		/// @~japanese 「Tuple」と表記されているが、実際は「(int feet, int inch)」フィート/インチ。@n
		/// @~
		/// </returns>
        public (int feet, int inch) ConvertMeterIntoFeetAndInch(float meter)
        {
            int feet = (int)(meter / ONE_FEET);
            int inch = (int)Math.Round((meter - feet * ONE_FEET) / ONE_INCH);

            return (feet,inch);
        }
    }

	/// <summary>
	/// @~japanese スケルトン定義データ構造体@n
	/// @~
	/// </summary>
	public struct SkeletonDefinitionData
	{
		/// <summary>
		/// @~japanese mocopiのボーンIDリスト@n
		/// @~
		/// </summary>
		public int[] BoneIds;

		/// <summary>
		/// @~japanese 各ボーンに対する親ボーンのIDリスト@n
		/// @~
		/// </summary>
		public int[] ParentBoneIds;

		/// <summary>
		/// @~japanese 各ボーンの初期回転角@n
		/// @~
		/// </summary>
		public float[] RotationsX;

		/// <summary>
		/// @~japanese 各ボーンの初期回転角@n
		/// @~
		/// </summary>
		public float[] RotationsY;

		/// <summary>
		/// @~japanese 各ボーンの初期回転角@n
		/// @~
		/// </summary>
		public float[] RotationsZ;

		/// <summary>
		/// @~japanese 各ボーンの初期回転角@n
		/// @~
		/// </summary>
		public float[] RotationsW;

		/// <summary>
		/// @~japanese 各ボーンの初期位置@n
		/// @~
		/// </summary>
		public float[] PositionsX;

		/// <summary>
		/// @~japanese 各ボーンの初期位置@n
		/// @~
		/// </summary>
		public float[] PositionsY;

		/// <summary>
		/// @~japanese 各ボーンの初期位置@n
		/// @~
		/// </summary>
		public float[] PositionsZ;
	}

	/// <summary>
	/// @~japanese スケルトンフレームデータ構造体@n
	/// @~
	/// </summary>
	public struct SkeletonData
	{
		/// <summary>
		/// @~japanese フレームID @n
		/// @~
		/// </summary>
		public int FrameId;

		/// <summary>
		/// @~japanese タイムスタンプ @n
		/// @~
		/// </summary>
		public double Timestamp;

		/// <summary>
		/// @~japanese mocopiのボーンIDリスト@n
		/// @~
		/// </summary>
		public int[] BoneIds;

		/// <summary>
		/// @~japanese 各ボーンの初期回転角@n
		/// @~
		/// </summary>
		public float[] RotationsX;

		/// <summary>
		/// @~japanese 各ボーンの初期回転角@n
		/// @~
		/// </summary>
		public float[] RotationsY;

		/// <summary>
		/// @~japanese 各ボーンの初期回転角@n
		/// @~
		/// </summary>
		public float[] RotationsZ;

		/// <summary>
		/// @~japanese 各ボーンの初期回転角@n
		/// @~
		/// </summary>
		public float[] RotationsW;

		/// <summary>
		/// @~japanese 各ボーンの初期位置@n
		/// @~
		/// </summary>
		public float[] PositionsX;

		/// <summary>
		/// @~japanese 各ボーンの初期位置@n
		/// @~
		/// </summary>
		public float[] PositionsY;

		/// <summary>
		/// @~japanese 各ボーンの初期位置@n
		/// @~
		/// </summary>
		public float[] PositionsZ;

		/// <summary>
		/// @~japanese アバターをミラーリングしているか@n
		/// @~
		/// </summary>
		public bool IsMirroing;
	}

	/// <summary>
	/// @~japanese BVH読み込み開始用構造体@n
	/// @~
	/// </summary>
	public struct MotionStreamingReadStartedData
	{
		/// <summary>
		/// @~japanese BVH読み込み開始データ@n
		/// @~
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="jointIds"></param>
		/// <param name="parentJointIds"></param>
		/// <param name="rotationsX"></param>
		/// <param name="rotationsY"></param>
		/// <param name="rotationsZ"></param>
		/// <param name="rotationsW"></param>
		/// <param name="positionsX"></param>
		/// <param name="positionsY"></param>
		/// <param name="positionsZ"></param>
		/// <param name="frames"></param>
		/// <param name="frameTime"></param>
		public MotionStreamingReadStartedData(
			string fileName,
			int[] jointIds, int[] parentJointIds,
			float[] rotationsX, float[] rotationsY, float[] rotationsZ, float[] rotationsW,
			float[] positionsX, float[] positionsY, float[] positionsZ,
			int frames, float frameTime)
		{
			this.FileName = fileName;
			this.ParentJointIds = parentJointIds;
			this.FrameData = new MotionStreamingFrameData(
				jointIds,
				rotationsX, rotationsY, rotationsZ, rotationsW,
				positionsX, positionsY, positionsZ);
			this.Frames = frames;
			this.FrameTime = frameTime;
		}

		/// <summary>
		/// @~japanese ファイル名@n
		/// @~
		/// </summary>
		public string FileName { get; set; }

		/// <summary>
		/// @~japanese ジョイントID@n
		/// @~
		/// </summary>
		public int[] ParentJointIds { get; set; }

		/// <summary>
		/// @~japanese フレームデータ@n
		/// @~
		/// </summary>
		public MotionStreamingFrameData FrameData { get; set; }

		/// <summary>
		/// @~japanese フレーム数@n
		/// @~
		/// </summary>
		public int Frames { get; set; }

		/// <summary>
		/// @~japanese フレーム時間@n
		/// @~
		/// </summary>
		public float FrameTime { get; set; }
	}

	/// <summary>
	/// @~japanese BVH読み込みフレーム構造体@n
	/// @~
	/// </summary>
	public struct MotionStreamingFrameData
	{
		/// <summary>
		/// @~japanese BVH読み込みフレームデータ@n
		/// @~
		/// </summary>
		/// <param name="jointIds"></param>
		/// <param name="rotationsX"></param>
		/// <param name="rotationsY"></param>
		/// <param name="rotationsZ"></param>
		/// <param name="rotationsW"></param>
		/// <param name="positionsX"></param>
		/// <param name="positionsY"></param>
		/// <param name="positionsZ"></param>
		public MotionStreamingFrameData(
			int[] jointIds,
			float[] rotationsX, float[] rotationsY, float[] rotationsZ, float[] rotationsW,
			float[] positionsX, float[] positionsY, float[] positionsZ)
		{
			this.JointIds = jointIds;
			this.RotationsX = rotationsX;
			this.RotationsY = rotationsY;
			this.RotationsZ = rotationsZ;
			this.RotationsW = rotationsW;
			this.PositionsX = positionsX;
			this.PositionsY = positionsY;
			this.PositionsZ = positionsZ;
		}

		/// <summary>
		/// @~japanese ジョイントID@n
		/// @~
		/// </summary>
		public int[] JointIds { get; set; }

		/// <summary>
		/// @~japanese ローテーションX@n
		/// @~
		/// </summary>
		public float[] RotationsX { get; set; }

		/// <summary>
		/// @~japanese ローテーションY@n
		/// @~
		/// </summary>
		public float[] RotationsY { get; set; }

		/// <summary>
		/// @~japanese ローテーションZ@n
		/// @~
		/// </summary>
		public float[] RotationsZ { get; set; }

		/// <summary>
		/// @~japanese ローテーションW@n
		/// @~
		/// </summary>
		public float[] RotationsW { get; set; }

		/// <summary>
		/// @~japanese ポジションX@n
		/// @~
		/// </summary>
		public float[] PositionsX { get; set; }

		/// <summary>
		/// @~japanese ポジションY@n
		/// @~
		/// </summary>
		public float[] PositionsY { get; set; }

		/// <summary>
		/// @~japanese ポジションZ@n
		/// @~
		/// </summary>
		public float[] PositionsZ { get; set; }
	}
}
