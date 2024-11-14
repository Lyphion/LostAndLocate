using AutoMapper;
using LostAndLocate.LostObjects.Configuration;
using LostAndLocate.LostObjects.Controllers;
using LostAndLocate.LostObjects.Services;
using LostAndLocate.Users.Configuration;
using NSubstitute;

namespace LostAndLocate.Test.UnitTests.LostObjects;

public class LostObjectControllerTest
{
    private readonly ILostObjectService _service = Substitute.For<ILostObjectService>();

    private readonly LostObjectController _controller;

    public LostObjectControllerTest()
    {
        var mapper = new MapperConfiguration(mc =>
        {
            mc.AddProfile<UserMappingProfile>();
            mc.AddProfile<LostObjectMappingProfile>();
        }).CreateMapper();
        
        _controller = new LostObjectController(_service, mapper);
    }

    #region GetObjects

    #endregion

    #region GetObject

    #endregion

    #region CreateObject

    #endregion

    #region UpdateObject

    #endregion

    #region DeleteObject

    #endregion
}