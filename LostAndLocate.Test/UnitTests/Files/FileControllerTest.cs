using LostAndLocate.Files.Controllers;
using LostAndLocate.Files.Services;
using NSubstitute;

namespace LostAndLocate.Test.UnitTests.Files;

public class FileControllerTest
{
    private readonly IFileService _service = Substitute.For<IFileService>();

    private readonly FileController _controller;

    public FileControllerTest()
    {
        _controller = new FileController(_service);
    }

    #region GetUserPicture

    #endregion

    #region GetObjectPicture

    #endregion

    #region UploadUserPicture

    #endregion

    #region UploadObjectPicture

    #endregion
}