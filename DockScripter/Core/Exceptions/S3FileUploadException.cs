namespace DockScripter.Core.Exceptions;

public class S3FileUploadException : Exception
{
    public S3FileUploadException() : base()
    {
    }

    public S3FileUploadException(string message) : base(message)
    {
    }

    public S3FileUploadException(string message, Exception innerException) : base(message, innerException)
    {
    }
}