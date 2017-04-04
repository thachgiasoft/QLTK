
using System;
namespace TPA.Model
{
	public class MaterialGroupModel : BaseModel
	{
		private int iD;
		private string code;
		private string name;
		private string createdBy;
		private DateTime? createdDate;
		private string updatedBy;
		private DateTime? updatedDate;
		private int parentID;
		private string description;
		public int ID
		{
			get { return iD; }
			set { iD = value; }
		}
	
		public string Code
		{
			get { return code; }
			set { code = value; }
		}
	
		public string Name
		{
			get { return name; }
			set { name = value; }
		}
	
		public string CreatedBy
		{
			get { return createdBy; }
			set { createdBy = value; }
		}
	
		public DateTime? CreatedDate
		{
			get { return createdDate; }
			set { createdDate = value; }
		}
	
		public string UpdatedBy
		{
			get { return updatedBy; }
			set { updatedBy = value; }
		}
	
		public DateTime? UpdatedDate
		{
			get { return updatedDate; }
			set { updatedDate = value; }
		}
	
		public int ParentID
		{
			get { return parentID; }
			set { parentID = value; }
		}
	
		public string Description
		{
			get { return description; }
			set { description = value; }
		}
	
	}
}
	