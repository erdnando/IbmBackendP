namespace Algar.Hours.Application.DataBase.Client.Commands.Update
{
	public interface IUpdateClientCommand
	{
		Task<Boolean> Update(ClientModel model);
	}
}
