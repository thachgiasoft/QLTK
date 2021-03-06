
using System;
namespace BMS.Model
{
	public class BaiThucHanhModuleModel : BaseModel
	{
		private int iD;
		private int baiThucHanhID;
		private string code;
		private string name;
		private int qty;
		private string hang;
		private int type;
		private int cVersion;
		private string createdBy;
		private DateTime? createdDate;
		private string updatedBy;
		private DateTime? updatedDate;
		public int ID
		{
			get { return iD; }
			set { iD = value; }
		}
	
		public int BaiThucHanhID
		{
			get { return baiThucHanhID; }
			set { baiThucHanhID = value; }
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
	
		public int Qty
		{
			get { return qty; }
			set { qty = value; }
		}
	
		public string Hang
		{
			get { return hang; }
			set { hang = value; }
		}
	
		public int Type
		{
			get { return type; }
			set { type = value; }
		}
	
		public int CVersion
		{
			get { return cVersion; }
			set { cVersion = value; }
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
	
	}
}
	