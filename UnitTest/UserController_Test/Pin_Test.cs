﻿using Bank.Api.Controllers;
using Bank.Core.Entity;
using Bank.Core.Interface;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest.UserController_Test
{
    [TestFixture]
    public class Pin_Test
    {
        private Pin _pin;
        private User _user;
        private List<User> _listuser;
        public static IConfiguration _config;
        public static IRepository _repo;
        private UserController controller;

        [SetUp]
        public void SetUp()
        {
            _config = A.Fake<IConfiguration>();
            _repo = A.Fake<IRepository>();

            controller = new UserController(_repo, _config);

            /// Set User List
            _listuser = GetDummyUser();

            /// Set User
            _user = new()
            {
                USERNAME = "dummyUser",
                NAME = "DUMMY DUMMY",
                PASSWORD = "DUMMY_DUMMY",
                MOTHER_MAIDEN_NAME = "MaidenName",
                ADDRESS = "DUMMY ADDRESS",
                KELURAHAN = "DUMMY LURAH",
                KECAMATAN = "DUMMY KECAMATAN",
                KABUPATEN_KOTA = "DUMMY KOTA",
                PROVINCE = "DUMMY PROVINCE",
                BIRTH_PLACE = "DUMMY CITY",
                EMAIL = "dummy1@dummy.com",
                GENDER = 'M',
                JOB = "PNS",
                PHONE = "08123456789",
                NIK = "1234567891012131",
                MARITAL_STATUS = "Lajang",
                FOTO_KTP_SELFIE = "DUMMY KTP LINK",
                VIDEO = "DUMMY VIDEO LINK",
                USER_TYPE = "user",
                BIRTH_DATE = new DateTime(2001, 03, 13), // 2001-03-13 00:00:00.000
                PIN = "f82c08b599cd4299be0edc2441c35add3b9b6df6"    // 624351
            };

            /// Set Pin
            _pin = new()
            {
                TOKEN = "",
                USERNAME = "dummyUser",
                PIN = "065314",
                NEW_PIN = "413560",
                user = new User(),
                mode = "create",
            };
        }

        [TestCase("200", "dummyUser")]  // PIN created successfully
        [TestCase("401", "dummyUserX")] // Username not registered.
        public void CreatePINTest(string resultCode, string param1)
        {
            #region Set Up Param
            var resultValue = "";
            /// Set Pin
            var pin = _pin;
            pin.USERNAME = param1;
            pin.mode = "create";

            /// Add list user
            A.CallTo(() => _repo.List<User>(null)).Returns(GetDummyUser());
            #endregion

            var result = controller.CreatePIN(_pin);

            if (resultCode == "401")
            {
                var x = result as UnauthorizedObjectResult;
                resultValue = x.StatusCode.ToString();
            }
            else if (resultCode == "200")
            {
                var x = result as OkObjectResult;
                resultValue = x.StatusCode.ToString();
            }

            Assert.AreEqual(resultCode, resultValue);
        }

        [TestCase("200", "dummyUser")]  // PIN created successfully
        [TestCase("401", "dummyUserX")] // Username not registered.
        public void ChangePINTest(string resultCode, string param1)
        {
            #region Set Up Param
            var resultValue = "";
            /// Set Pin
            var pin = _pin;
            pin.USERNAME = param1;
            pin.mode = "create";

            /// Add list user
            A.CallTo(() => _repo.List<User>(null)).Returns(GetDummyUser());
            A.CallTo(() => _repo.List<RefMaster>(null)).Returns(GetRefMasters());
            #endregion

            var result = controller.ChangePIN(_pin);

            if (resultCode == "401")
            {
                var x = result as UnauthorizedObjectResult;
                resultValue = x.StatusCode.ToString();
            }
            else if (resultCode == "200")
            {
                var x = result as OkObjectResult;
                resultValue = x.StatusCode.ToString();
            }

            Assert.AreEqual(resultCode, resultValue);
        }

        [TestCase("200", "dummyUser")]  // PIN created successfully
        [TestCase("401", "dummyUserX")] // Username not registered.
        public void PINStatus(string resultCode, string param1)
        {
            #region Set Up Param
            var resultValue = "";
            /// Set Pin
            var pin = _pin;
            pin.USERNAME = param1;
            pin.mode = "status";

            /// Add list user
            A.CallTo(() => _repo.List<User>(null)).Returns(GetDummyUser());
            #endregion

            var result = controller.PINStatus(_pin);

            if (resultCode == "401")
            {
                var x = result as UnauthorizedObjectResult;
                resultValue = x.StatusCode.ToString();
            }
            else if (resultCode == "200")
            {
                var x = result as OkObjectResult;
                resultValue = x.StatusCode.ToString();
            }

            Assert.AreEqual(resultCode, resultValue);
        }

        [TestCase("create", "", "080389")]
        [TestCase("create", "Invalid pin", null)]
        [TestCase("create", "Invalid input. Only accept 6 digit numbers", "")]
        [TestCase("create", "Only accept 6 digit numbers", "12345y")]
        [TestCase("create", "Pin too short. Only accept 6 digit numbers", "123")]
        [TestCase("create", "Pin too long. Only accept 6 digit numbers", "1234567")]
        [TestCase("create", "Please do not use your Date of Birth (DOB)", "130301")]
        [TestCase("create", "Please do not use your Date of Birth (DOB)", "010313")]
        [TestCase("create", "Please DO NOT use \"123456\" or \"654321\" and the other general number as your pin", "123456")]
        [TestCase("create", "Please DO NOT use \"123456\" or \"654321\" and the other general number as your pin", "654321")]
        [TestCase("change", "New pin must be different from the old one.", "624351")]
        [TestCase("change", "", "065314")]
        [TestCase("change", "Invalid pin", null)]
        [TestCase("change", "Invalid input. Only accept 6 digit numbers", "")]
        [TestCase("change", "Only accept 6 digit numbers", "12345y")]
        [TestCase("change", "Pin too short. Only accept 6 digit numbers", "123")]
        [TestCase("change", "Pin too long. Only accept 6 digit numbers", "1234567")]
        [TestCase("change", "Please do not use your Date of Birth (DOB)", "130301")]
        [TestCase("change", "Please do not use your Date of Birth (DOB)", "010313")]
        [TestCase("change", "Please DO NOT use \"123456\" or \"654321\" and the other general number as your pin", "123456")]
        [TestCase("change", "Please DO NOT use \"123456\" or \"654321\" and the other general number as your pin", "654321")]
        public void ValidatePIN(string mode, string expected, string param1)
        {
            // ARRANGE
            _pin.mode = mode;
            _pin.PIN = param1;
            _pin.NEW_PIN = param1;
            _pin.user = _user;
            /// Add list
            A.CallTo(() => _repo.List<RefMaster>(null)).Returns(GetRefMasters());
            A.CallTo(() => _repo.List<User>(null)).Returns(GetDummyUser());
            // ACT
            var result = controller.ValidatePIN(_pin);
            // ASSERT
            Assert.AreEqual(result, expected);
        }

        private List<RefMaster> GetRefMasters()
        {
            List<RefMaster> refMasters = new()
            {
                new RefMaster
                {
                    ID = 34,
                    MASTER_GROUP = "PIN",
                    MASTER_CODE = "VALIDATION",
                    MASTER_CODE_DESCRIPTION = "Invalid pin",
                    VALUE = null,
                    IS_ACTIVE = true,
                },
                new RefMaster
                {
                    ID = 35,
                    MASTER_GROUP = "PIN",
                    MASTER_CODE = "VALIDATION",
                    MASTER_CODE_DESCRIPTION = "Invalid input. Only accept 6 digit numbers",
                    VALUE = "",
                    IS_ACTIVE = true,
                },
                new RefMaster
                {
                    ID = 36,
                    MASTER_GROUP = "PIN",
                    MASTER_CODE = "FORMAT",
                    MASTER_CODE_DESCRIPTION = "Please do not use your Date of Birth (DOB)",
                    VALUE = "yyMMdd",
                    IS_ACTIVE = true,
                },
                new RefMaster
                {
                    ID = 37,
                    MASTER_GROUP = "PIN",
                    MASTER_CODE = "FORMAT",
                    MASTER_CODE_DESCRIPTION = "Please do not use your Date of Birth (DOB)",
                    VALUE = "ddMMyy",
                    IS_ACTIVE = true,
                },
                new RefMaster
                {
                    ID = 38,
                    MASTER_GROUP = "PIN",
                    MASTER_CODE = "FORMAT",
                    MASTER_CODE_DESCRIPTION = "Please do not use your Date of Birth (DOB)",
                    VALUE = "MMyyyy",
                    IS_ACTIVE = true,
                },
                new RefMaster
                {
                    ID = 39,
                    MASTER_GROUP = "PIN",
                    MASTER_CODE = "FORMAT",
                    MASTER_CODE_DESCRIPTION = "Please do not use your Date of Birth (DOB)",
                    VALUE = "yyyyMM",
                    IS_ACTIVE = true,
                },
                new RefMaster
                {
                    ID = 40,
                    MASTER_GROUP = "PIN",
                    MASTER_CODE = "FORMAT",
                    MASTER_CODE_DESCRIPTION = "Please DO NOT use \"123456\" or \"654321\" and the other general number as your pin",
                    VALUE = "123456",
                    IS_ACTIVE = true,
                },
                new RefMaster
                {
                    ID = 41,
                    MASTER_GROUP = "PIN",
                    MASTER_CODE = "FORMAT",
                    MASTER_CODE_DESCRIPTION = "Please DO NOT use \"123456\" or \"654321\" and the other general number as your pin",
                    VALUE = "654321",
                    IS_ACTIVE = true,
                },
                new RefMaster
                {
                    ID = 42,
                    MASTER_GROUP = "PIN",
                    MASTER_CODE = "LENGTH",
                    MASTER_CODE_DESCRIPTION = "PIN Length",
                    VALUE = "6",
                    IS_ACTIVE = true,
                }
            };

            return refMasters;
        }

        private List<User> GetDummyUser()
        {
            List<User> dummy = new List<User>()
            {
                new User{
                    USERNAME = "dummyUser",
                    NAME = "DUMMY DUMMY",
                    PASSWORD = "DUMMY_DUMMY",
                    MOTHER_MAIDEN_NAME = "MaidenName",
                    ADDRESS = "DUMMY ADDRESS",
                    KELURAHAN = "DUMMY LURAH",
                    KECAMATAN="DUMMY KECAMATAN",
                    KABUPATEN_KOTA = "DUMMY KOTA",
                    PROVINCE = "DUMMY PROVINCE",
                    BIRTH_PLACE = "DUMMY CITY",
                    EMAIL = "dummy1@dummy.com",
                    GENDER = 'M',
                    JOB = "PNS",
                    PHONE = "08123456789",
                    NIK = "1234567891012131",
                    MARITAL_STATUS = "Lajang",
                    FOTO_KTP_SELFIE = "DUMMY KTP LINK",
                    VIDEO = "DUMMY VIDEO LINK",
                    USER_TYPE = "user",
                    BIRTH_DATE = new DateTime(2000, 03, 13) // 2000-03-13 00:00:00.000
                },
                new User{
                    USERNAME = "dummyUser1",
                    NAME = "DUMMY KEDUA KAKA",
                    PASSWORD = "DUMMY_DUMMY",
                    MOTHER_MAIDEN_NAME = "MaidenName",
                    ADDRESS = "DUMMY ADDRESS",
                    KELURAHAN = "DUMMY LURAH",
                    KECAMATAN="DUMMY KECAMATAN",
                    KABUPATEN_KOTA = "DUMMY KOTA",
                    PROVINCE = "DUMMY PROVINCE",
                    BIRTH_PLACE = "DUMMY CITY",
                    EMAIL = "dummy2@dummy.com",
                    GENDER = 'M',
                    JOB = "PNS",
                    PHONE = "08123456789",
                    NIK = "1234567891012131",
                    MARITAL_STATUS = "Lajang",
                    FOTO_KTP_SELFIE = "DUMMY KTP LINK",
                    VIDEO = "DUMMY VIDEO LINK",
                    USER_TYPE = "user"
                },
                new User{
                    USERNAME = "dummyUser2",
                    NAME = "DUMMY KETIGA",
                    PASSWORD = "DUMMY_DUMMY",
                    MOTHER_MAIDEN_NAME = "MaidenName",
                    ADDRESS = "DUMMY ADDRESS",
                    KELURAHAN = "DUMMY LURAH",
                    KECAMATAN="DUMMY KECAMATAN",
                    KABUPATEN_KOTA = "DUMMY KOTA",
                    PROVINCE = "DUMMY PROVINCE",
                    BIRTH_PLACE = "DUMMY CITY",
                    EMAIL = "dummy3@dummy.com",
                    GENDER = 'M',
                    JOB = "PNS",
                    PHONE = "08123456789",
                    NIK = "1234567891012131",
                    MARITAL_STATUS = "Lajang",
                    FOTO_KTP_SELFIE = "DUMMY KTP LINK",
                    VIDEO = "DUMMY VIDEO LINK",
                    USER_TYPE = "user"
                },
        };
            return dummy;
        }
    }
}
