﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuelSDK.Test
{
    class ETTriggeredSendDefinitionTest
    {
        ETClient client;
        ETTriggeredSendDefinition tsd;
        ETEmail email;
        string tsdName;
        string desc;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            client = new ETClient();
        }

        [SetUp]
        public void Setup()
        {
            tsdName = Guid.NewGuid().ToString();
            desc = "Triggered Send Definition created by C# SDK";
            var emailName = string.Empty;
            var emailCustKey = emailName = System.Guid.NewGuid().ToString();
            var emailContent = "<b>This is a content generated by Fuel SDK C#";

            var emailObj = new ETEmail
            {
                AuthStub = client,
                Name = emailName,
                CustomerKey = emailCustKey,
                Subject = "This email is created using C# SDK",
                HTMLBody = emailContent
            };
            var emailResponse = emailObj.Post();
            Assert.AreEqual(emailResponse.Code, 200);
            Assert.AreEqual(emailResponse.Status, true);
            email = (ETEmail)emailResponse.Results[0].Object;

            var tsdObj = new ETTriggeredSendDefinition
            {
                AuthStub = client,
                Name = tsdName,
                Description = desc,
                CustomerKey = tsdName,
                Email = email,
                SendClassification = new ETSendClassification { CustomerKey = "Default Commercial" },
            };

            var response = tsdObj.Post();
            Assert.AreEqual(response.Code, 200);
            Assert.AreEqual(response.Status, true);
            Assert.AreEqual(response.Results[0].StatusMessage, "TriggeredSendDefinition created");
            tsd = (ETTriggeredSendDefinition)response.Results[0].Object;
        }

        [TearDown]
        public void TearDown()
        {
            var tsdObj = new ETTriggeredSendDefinition
            {
                AuthStub = client,
                CustomerKey = tsdName
            };
            var response = tsdObj.Delete();

            var email = new ETEmail
            {
                AuthStub = client,
                CustomerKey = this.email.CustomerKey
            };
            var emailResponse = email.Delete();
        }

        [Test()]
        public void TriggeredSendDefinitionCreate()
        {
            Assert.AreNotEqual(tsd, null);
        }

        [Test()]
        public void TriggeredSendDefinitionGet()
        {
            var tsdObj = new ETTriggeredSendDefinition
            {
                AuthStub = client,
                CustomerKey = tsdName,
                Props = new[] { "Name", "CustomerKey", "Description" },
                SearchFilter = new SimpleFilterPart { Property = "CustomerKey", SimpleOperator = SimpleOperators.equals, Value = new[] { tsd.CustomerKey } }
            };
            var response = tsdObj.Get();
            Assert.AreEqual(response.Code, 200);
            Assert.AreEqual(response.Status, true);
            tsdObj = (ETTriggeredSendDefinition)response.Results[0];
            Assert.AreEqual(tsd.Description, tsdObj.Description);
        }

        [Test()]
        public void TriggeredSendDefinitionUpdate()
        {
            var updatedDesc = "Updated TSD";
            var tsdObj = new ETTriggeredSendDefinition
            {
                AuthStub = client,
                Description = updatedDesc,
                CustomerKey = tsdName,
                Email = email
            };

            var response = tsdObj.Patch();
            Assert.AreEqual(response.Code, 200);
            Assert.AreEqual(response.Status, true);
            Assert.AreEqual(response.Results[0].StatusMessage, "TriggeredSendDefinition updated");

            tsdObj = new ETTriggeredSendDefinition
            {
                AuthStub = client,
                CustomerKey = tsdName,
                Props = new[] { "Name", "CustomerKey", "Description" },
                SearchFilter = new SimpleFilterPart { Property = "CustomerKey", SimpleOperator = SimpleOperators.equals, Value = new[] { tsd.CustomerKey } }
            };
            var getResponse = tsdObj.Get();
            Assert.AreEqual(getResponse.Code, 200);
            Assert.AreEqual(getResponse.Status, true);
            tsdObj = (ETTriggeredSendDefinition)getResponse.Results[0];
            Assert.AreEqual(updatedDesc, tsdObj.Description);
        }

        [Test()]
        public void TriggeredSendDefinitionDelete()
        {
            var tsdObj = new ETTriggeredSendDefinition
            {
                AuthStub = client,
                CustomerKey = tsdName
            };

            var response = tsdObj.Delete();
            Assert.AreEqual(response.Code, 200);
            Assert.AreEqual(response.Status, true);
            Assert.AreEqual(response.Results[0].StatusMessage, "TriggeredSendDefinition deleted");
        }

        [Test()]
        public void TriggeredSendDefinitionSend()
        {
            var tsdObj = new ETTriggeredSendDefinition
            {
                AuthStub = client,
                CustomerKey = tsdName,
                TriggeredSendStatus = TriggeredSendStatusEnum.Active
            };
            var updResponse = tsdObj.Patch();
            Assert.AreEqual(updResponse.Code, 200);
            Assert.AreEqual(updResponse.Status, true);

            tsdObj = new ETTriggeredSendDefinition
            {
                AuthStub = client,
                CustomerKey = tsdName,
                Subscribers = new[] { new ETSubscriber { EmailAddress = "customer@youremail.com", SubscriberKey = "customer@youremail.com" } },
            };

            var response = tsdObj.Send();
            Assert.AreEqual(response.Code, 200);
            Assert.AreEqual(response.Status, true);
            Assert.AreEqual(response.Results[0].StatusMessage, "Created TriggeredSend");
        }
    }
}
