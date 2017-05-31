namespace WebApiService.Diagnostics
{
	public interface IWebApiLogger
	{
		void ActivatingController();
	    void StartGetAll();
	    void StopGetAll();
	}
}