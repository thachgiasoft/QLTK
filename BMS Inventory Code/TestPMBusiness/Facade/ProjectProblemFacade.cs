
using System.Collections;
using TEST.Model;
namespace TEST.Facade
{
	
	public class ProjectProblemFacade : BaseFacade
	{
		protected static ProjectProblemFacade instance = new ProjectProblemFacade(new ProjectProblemModel());
		protected ProjectProblemFacade(ProjectProblemModel model) : base(model)
		{
		}
		public static ProjectProblemFacade Instance
		{
			get { return instance; }
		}
		protected ProjectProblemFacade():base() 
		{ 
		} 
	
	}
}
	