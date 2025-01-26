namespace Server.DataApplicationLayer.Messages.Exception
{
	public class NotFoundException(string? name, object key)
		: System.Exception($"\"{name}\" ({key}): not found.");
}
