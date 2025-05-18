using AutoMapper;
using Moq;
using WebApi.Commands.Instance;
using WebApi.DtoModels.ContactInfo;
using WebApi.Models;
using WebApi.Profiles;
using WebApi.Services.Interface;
using static WebApi.Models.ContactInfo;

namespace WebApiTest.Commands
{
    public class ContactInfoCommandTest
    {
        private readonly IMapper _mapper;

        public ContactInfoCommandTest()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ContactInfoProfile>();
            });
            _mapper = config.CreateMapper();
        }

        [Theory]
        [InlineData(1, 0)]
        [InlineData(2, 0)]
        [InlineData(3, 0)]
        public void QueryByID_Success(long id, int expected)
        {
            #region Arrange
            var mockResult = new ContactInfo
            {
                ContactInfoID = id,
                Name = "Test",
                Nickname = "T",
                PhoneNo = "123456",
                Address = "TestAddress",
                IsEnable = true,
                CreateTime = DateTime.Now.AddMinutes(-5)
            };

            var mockService = new Mock<IContactInfoService>();
            mockService.Setup(e => e.Query(It.IsAny<long>())).Returns(mockResult);
            #endregion

            #region Act
            var processor = new ContactInfoCommand(_mapper, mockService.Object);
            var result = processor.QueryByID(id);
            #endregion

            #region Assert
            Assert.Equal(expected, result.Code);
            mockService.Verify(e => e.Query(It.IsAny<long>()), Times.Once);
            #endregion
        }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(-1, 1)]
        [InlineData(999, 1)]
        public void QueryByID_Fail(long id, int expected)
        {
            #region Arrange
            var mockResult = (ContactInfo)null;

            var mockService = new Mock<IContactInfoService>();
            mockService.Setup(e => e.Query(It.IsAny<long>())).Returns(mockResult);
            #endregion

            #region Act
            var processor = new ContactInfoCommand(_mapper, mockService.Object);
            var result = processor.QueryByID(id);
            #endregion

            #region Assert
            Assert.Equal(expected, result.Code);
            mockService.Verify(e => e.Query(It.IsAny<long>()), Times.Once);
            #endregion
        }

        [Theory]
        [MemberData(nameof(ContactInfoCommandTestData.QueryRQs_Success), MemberType = typeof(ContactInfoCommandTestData))]
        public void QueryByCondition_Success(QueryRQ objRQ, int expected)
        {
            #region Arrange
            var mockData = new List<ContactInfo>();
            for (int i=0; i<10; i++)
            {
                mockData.Add(new ContactInfo
                {
                    ContactInfoID = i+1,
                    Name = "Test",
                    Nickname = "T",
                    Gender = ContactInfo.EnumGender.Male,
                    PhoneNo = "123456",
                    Address = "TestAddress",
                    IsEnable = true,
                    CreateTime = DateTime.Now.AddMinutes(i-10)
                });
            }
            var mockResult = (10, mockData);

            var mockService = new Mock<IContactInfoService>();
            mockService.Setup(e => e.Query(It.IsAny<Dictionary<string, object>>())).Returns(mockResult);
            #endregion

            #region Act
            var processor = new ContactInfoCommand(_mapper, mockService.Object);
            var result = processor.QueryByCondition(objRQ);
            #endregion

            #region Assert
            Assert.Equal(expected, result.Code);
            mockService.Verify(e => e.Query(It.IsAny<Dictionary<string, object>>()), Times.Once);
            #endregion
        }

        [Theory]
        [MemberData(nameof(ContactInfoCommandTestData.QueryRQs_Fail), MemberType = typeof(ContactInfoCommandTestData))]
        public void QueryByCondition_Fail(QueryRQ objRQ, int expected)
        {
            #region Arrange
            var mockResult = (0, (List<ContactInfo>)null);

            var mockService = new Mock<IContactInfoService>();
            mockService.Setup(e => e.Query(It.IsAny<Dictionary<string, object>>())).Returns(mockResult);
            #endregion

            #region Act
            var processor = new ContactInfoCommand(_mapper, mockService.Object);
            var result = processor.QueryByCondition(objRQ);
            #endregion

            #region Assert
            Assert.Equal(expected, result.Code);
            mockService.Verify(e => e.Query(It.IsAny<Dictionary<string, object>>()), Times.Once);
            #endregion
        }

        [Theory]
        [MemberData(nameof(ContactInfoCommandTestData.CreateRQs_Success), MemberType = typeof(ContactInfoCommandTestData))]
        public void Create_Success(CreateRQ objRQ, int expected)
        {
            #region Arrange
            var mockInsertResult = true;
            var mockQueryResult = new ContactInfo
            {
                ContactInfoID = new Random().Next(1, 100),
                Name = objRQ.Name,
                Nickname = objRQ.Nickname,
                Gender = (EnumGender?)objRQ.Gender,
                Age = objRQ.Age,
                PhoneNo = objRQ.PhoneNo,
                Address = objRQ.Address,
                IsEnable = true,
                CreateTime = DateTime.Now.AddMinutes(-5)
            };

            var mockService = new Mock<IContactInfoService>();
            mockService.Setup(e => e.Insert(It.IsAny<ContactInfo>())).Returns(mockInsertResult);
            mockService.Setup(e => e.Query(It.IsAny<long>())).Returns(mockQueryResult);
            #endregion

            #region Act
            var processor = new ContactInfoCommand(_mapper, mockService.Object);
            var result = processor.Create(objRQ);
            #endregion

            #region Assert
            Assert.Equal(expected, result.Code);
            mockService.Verify(e => e.Insert(It.IsAny<ContactInfo>()), Times.Once);
            #endregion
        }

        [Theory]
        [MemberData(nameof(ContactInfoCommandTestData.CreateRQs_Fail), MemberType = typeof(ContactInfoCommandTestData))]
        public void Create_Fail(CreateRQ objRQ, int expected)
        {
            #region Arrange
            var mockInsertResult = false;

            var mockService = new Mock<IContactInfoService>();
            mockService.Setup(e => e.Insert(It.IsAny<ContactInfo>())).Returns(mockInsertResult);
            #endregion

            #region Act
            var processor = new ContactInfoCommand(_mapper, mockService.Object);
            var result = processor.Create(objRQ);
            #endregion

            #region Assert
            Assert.Equal(expected, result.Code);
            mockService.Verify(e => e.Insert(It.IsAny<ContactInfo>()), Times.Once);
            #endregion
        }

        [Theory]
        [MemberData(nameof(ContactInfoCommandTestData.EditRQs_Success), MemberType = typeof(ContactInfoCommandTestData))]
        public void Edit_Success(EditRQ objRQ, int expected)
        {
            #region Arrange
            var mockUpdateResult = true;
            var mockQueryResult = new ContactInfo
            {
                ContactInfoID = objRQ.ID.Value,
                Name = objRQ.Name,
                Nickname = objRQ.Nickname,
                Gender = (EnumGender?)objRQ.Gender,
                Age = objRQ.Age,
                PhoneNo = objRQ.PhoneNo,
                Address = objRQ.Address,
                IsEnable = true,
                CreateTime = DateTime.Now.AddMinutes(-5)
            };

            var mockService = new Mock<IContactInfoService>();
            mockService.Setup(e => e.Update(It.IsAny<ContactInfo>())).Returns(mockUpdateResult);
            mockService.Setup(e => e.Query(It.IsAny<long>())).Returns(mockQueryResult);
            #endregion

            #region Act
            var processor = new ContactInfoCommand(_mapper, mockService.Object);
            var result = processor.Edit(objRQ);
            #endregion

            #region Assert
            Assert.Equal(expected, result.Code);
            mockService.Verify(e => e.Update(It.IsAny<ContactInfo>()), Times.Once);
            #endregion
        }

        [Theory]
        [MemberData(nameof(ContactInfoCommandTestData.EditRQs_Fail), MemberType = typeof(ContactInfoCommandTestData))]
        public void Edit_Fail(EditRQ objRQ, int expected)
        {
            #region Arrange
            var mockUpdateResult = false;
            var mockQueryResult = new ContactInfo
            {
                ContactInfoID = objRQ.ID.Value,
                Name = objRQ.Name,
                Nickname = objRQ.Nickname,
                Gender = (EnumGender?)objRQ.Gender,
                Age = objRQ.Age,
                PhoneNo = objRQ.PhoneNo,
                Address = objRQ.Address,
                IsEnable = true,
                CreateTime = DateTime.Now.AddMinutes(-5)
            };

            var mockService = new Mock<IContactInfoService>();
            mockService.Setup(e => e.Update(It.IsAny<ContactInfo>())).Returns(mockUpdateResult);
            mockService.Setup(e => e.Query(It.IsAny<long>())).Returns(mockQueryResult);
            #endregion

            #region Act
            var processor = new ContactInfoCommand(_mapper, mockService.Object);
            var result = processor.Edit(objRQ);
            #endregion

            #region Assert
            Assert.Equal(expected, result.Code);
            mockService.Verify(e => e.Update(It.IsAny<ContactInfo>()), Times.Once);
            #endregion
        }

        [Theory]
        [MemberData(nameof(ContactInfoCommandTestData.EditPartialRQs_Success), MemberType = typeof(ContactInfoCommandTestData))]
        public void EditPartial_Success(EditPartialRQ objRQ, int expected)
        {
            #region Arrange
            var mockUpdateResult = true;
            var mockQueryResult = new ContactInfo
            {
                ContactInfoID = objRQ.ID.Value,
                Name = objRQ.Name,
                Nickname = objRQ.Nickname ?? $"T{objRQ.ID.Value}",
                Gender = (EnumGender?)objRQ.Gender ?? EnumGender.Male,
                Age = objRQ.Age ?? (short)new Random().Next(1, 100),
                PhoneNo = objRQ.PhoneNo,
                Address = objRQ.Address,
                IsEnable = true,
                CreateTime = DateTime.Now.AddMinutes(-5)
            };

            var mockService = new Mock<IContactInfoService>();
            mockService.Setup(e => e.Update(It.IsAny<ContactInfo>())).Returns(mockUpdateResult);
            mockService.Setup(e => e.Query(It.IsAny<long>())).Returns(mockQueryResult);
            #endregion

            #region Act
            var processor = new ContactInfoCommand(_mapper, mockService.Object);
            var result = processor.EditPartial(objRQ);
            #endregion

            #region Assert
            Assert.Equal(expected, result.Code);
            mockService.Verify(e => e.Update(It.IsAny<ContactInfo>()), Times.Once);
            #endregion
        }

        [Theory]
        [MemberData(nameof(ContactInfoCommandTestData.EditPartialRQs_Fail), MemberType = typeof(ContactInfoCommandTestData))]
        public void EditPartial_Fail(EditPartialRQ objRQ, int expected)
        {
            #region Arrange
            var mockUpdateResult = false;
            var mockQueryResult = new ContactInfo
            {
                ContactInfoID = objRQ.ID.Value,
                Name = objRQ.Name,
                Nickname = objRQ.Nickname ?? $"T{objRQ.ID.Value}",
                Gender = (EnumGender?)objRQ.Gender ?? EnumGender.Male,
                Age = objRQ.Age ?? (short)new Random().Next(1, 100),
                PhoneNo = objRQ.PhoneNo,
                Address = objRQ.Address,
                IsEnable = true,
                CreateTime = DateTime.Now.AddMinutes(-5)
            };

            var mockService = new Mock<IContactInfoService>();
            mockService.Setup(e => e.Update(It.IsAny<ContactInfo>())).Returns(mockUpdateResult);
            mockService.Setup(e => e.Query(It.IsAny<long>())).Returns(mockQueryResult);
            #endregion

            #region Act
            var processor = new ContactInfoCommand(_mapper, mockService.Object);
            var result = processor.EditPartial(objRQ);
            #endregion

            #region Assert
            Assert.Equal(expected, result.Code);
            mockService.Verify(e => e.Update(It.IsAny<ContactInfo>()), Times.Once);
            #endregion
        }

        [Theory]
        [MemberData(nameof(ContactInfoCommandTestData.RemoveIds_Success), MemberType = typeof(ContactInfoCommandTestData))]
        public  void Remove_Success(IEnumerable<long> ids, int expected)
        {
            #region Arrange
            var mockResult = true;

            var mockService = new Mock<IContactInfoService>();
            mockService.Setup(e => e.Delete(It.IsAny<IEnumerable<long>>())).Returns(mockResult);
            #endregion

            #region Act
            var processor = new ContactInfoCommand(_mapper, mockService.Object);
            var result = processor.Remove(ids);
            #endregion

            #region Assert
            Assert.Equal(expected, result.Code);
            mockService.Verify(e => e.Delete(It.IsAny<IEnumerable<long>>()), Times.Once);
            #endregion
        }

        [Theory]
        [MemberData(nameof(ContactInfoCommandTestData.RemoveIds_Fail), MemberType = typeof(ContactInfoCommandTestData))]
        public void Remove_Fail(IEnumerable<long> ids, int expected)
        {
            #region Arrange
            var mockResult = false;

            var mockService = new Mock<IContactInfoService>();
            mockService.Setup(e => e.Delete(It.IsAny<IEnumerable<long>>())).Returns(mockResult);
            #endregion

            #region Act
            var processor = new ContactInfoCommand(_mapper, mockService.Object);
            var result = processor.Remove(ids);
            #endregion

            #region Assert
            Assert.Equal(expected, result.Code);
            mockService.Verify(e => e.Delete(It.IsAny<IEnumerable<long>>()), Times.Once);
            #endregion
        }
    }

    public static class ContactInfoCommandTestData
    {
        public static IEnumerable<object[]> QueryRQs_Success => new List<object[]>
        {
            new object[] { new QueryRQ { PageIndex = 1, PageSize = 10 }, 0 },
            new object[] { new QueryRQ { PageIndex = 1, PageSize = 10, Gender = 1 }, 0 },
            new object[] { new QueryRQ { PageIndex = 1, PageSize = 10, Name = "Test" }, 0 }
        };

        public static IEnumerable<object[]> QueryRQs_Fail => new List<object[]>
        {
            new object[] { new QueryRQ { PageIndex = 0, PageSize = 10 }, 1 },
            new object[] { new QueryRQ { PageIndex = 1, PageSize = 10, Gender = 0 }, 1 },
            new object[] { new QueryRQ { PageIndex = 1, PageSize = 10, Name = "test" }, 1 }
        };

        public static IEnumerable<object[]> CreateRQs_Success => new List<object[]>
        {
            new object[] { new CreateRQ { Name = "Test", PhoneNo = "111111", Address = "TestAddress" }, 0 },
            new object[] { new CreateRQ { Name = "Test2", Nickname = "T2", PhoneNo = "222222", Address = "Test2Address" }, 0 },
            new object[] { new CreateRQ { Name = "Test3", Nickname = "T3", Gender = 1, Age = 3, PhoneNo = "333333", Address = "Test3Address" }, 0 }
        };

        public static IEnumerable<object[]> CreateRQs_Fail => new List<object[]>
        {
            new object[] { new CreateRQ { Name = "Test" }, 2 },
            new object[] { new CreateRQ { Name = "Test2", PhoneNo = "222222" }, 2 },
            new object[] { new CreateRQ { Name = "Test3", Address = "Test3Address" }, 2 }
        };

        public static IEnumerable<object[]> EditRQs_Success => new List<object[]>
        {
            new object[] { new EditRQ { ID = 1, Name = "Test", PhoneNo = "111111", Address = "TestAddress" }, 0 },
            new object[] { new EditRQ { ID = 2, Name = "Test2", Nickname = "T2", PhoneNo = "222222", Address = "Test2Address" }, 0 },
            new object[] { new EditRQ { ID = 3, Name = "Test3", Nickname = "T3", Gender = 1, Age = 3, PhoneNo = "333333", Address = "Test3Address" }, 0 }
        };

        public static IEnumerable<object[]> EditRQs_Fail => new List<object[]>
        {
            new object[] { new EditRQ { ID = 1, Name = "Test" }, 3 },
            new object[] { new EditRQ { ID = 2, Name = "Test2", Nickname = "T2", PhoneNo = "222222" }, 3 },
            new object[] { new EditRQ { ID = 3, Name = "Test3", Nickname = "T3", Gender = 1, Age = 3, Address = "Test3Address" }, 3 }
        };

        public static IEnumerable<object[]> EditPartialRQs_Success => new List<object[]>
        {
            new object[] { new EditPartialRQ { ID = 1, Name = "Test", PhoneNo = "111111", Address = "TestAddress" }, 0 },
            new object[] { new EditPartialRQ { ID = 2, Name = "Test2", Nickname = "T2", PhoneNo = "222222", Address = "Test2Address" }, 0 },
            new object[] { new EditPartialRQ { ID = 3, Name = "Test3", Nickname = "T3", Gender = 1, Age = 3, PhoneNo = "333333", Address = "Test3Address" }, 0 }
        };

        public static IEnumerable<object[]> EditPartialRQs_Fail => new List<object[]>
        {
            new object[] { new EditPartialRQ { ID = 1, Name = "Test" }, 3 },
            new object[] { new EditPartialRQ { ID = 2, Name = "Test2", Nickname = "T2", PhoneNo = "222222" }, 3 },
            new object[] { new EditPartialRQ { ID = 3, Name = "Test3", Nickname = "T3", Gender = 1, Age = 3, Address = "Test3Address" }, 3 }
        };

        public static IEnumerable<object[]> RemoveIds_Success => new List<object[]>
        {
            new object[] { new List<long> { 1 }, 0 },
            new object[] { new List<long> { 2, 3 }, 0 },
            new object[] { new List<long> { 5, 7, 9 }, 0 }
        };

        public static IEnumerable<object[]> RemoveIds_Fail => new List<object[]>
        {
            new object[] { new List<long> { 0 }, 4 },
            new object[] { new List<long> { -2, -3 }, 4 },
            new object[] { new List<long> { 95, 97, 99 }, 4 }
        };
    }
}
