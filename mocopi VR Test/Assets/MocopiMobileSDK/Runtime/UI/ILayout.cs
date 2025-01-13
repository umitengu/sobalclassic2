/*
* Copyright 2022 Sony Corporation
*/
namespace Mocopi.Ui
{
	/// <summary>
	/// Interface of layout infomation.
	/// </summary>
	public interface ILayout
	{
		/// <summary>
		/// Processing when creating an object instance.
		/// </summary>
		void Awake();

		/// <summary>
		/// Change to vertical layout.
		/// </summary>
		void ChangeToVerticalLayout();

		/// <summary>
		/// Change to horizontal layout.
		/// </summary>
		void ChangeToHorizontalLayout();
	}
}