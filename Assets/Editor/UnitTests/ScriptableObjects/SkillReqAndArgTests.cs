using NUnit.Framework;
using NSubstitute;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

namespace SkillReqAndArgTests {

    [TestFixture]
	internal class SkillReqAndArgTests {

        SkillReqAndArg testReq;

        [SetUp]
        public void Init() {
            testReq = ScriptableObject.CreateInstance<SkillReqAndArg>();
            testReq.UnitTesting_SetElemReq("metal", 5);
            testReq.UnitTesting_SetElemReq("wood", 5);
            testReq.UnitTesting_SetElemReq("water", 5);
            testReq.UnitTesting_SetElemReq("fire", 5);
            testReq.UnitTesting_SetElemReq("earth", 5);
            testReq.OnEnable();
            testReq.InitRequirements();
        }

        [Test]
        public void ReqInitTest() {
            Assert.That(testReq.UnitTesting_IsInitialized());
        }

        [Test]
        public void ElemRequirementTest() {
            Dictionary<EElements, int> elemReq = testReq.ElemRequirement();
            Assert.That(elemReq.Count == 5);
            Assert.That(elemReq[EElements.METAL] == 5);
            Assert.That(elemReq[EElements.WOOD] == 5);
            Assert.That(elemReq[EElements.WATER] == 5);
            Assert.That(elemReq[EElements.FIRE] == 5);
            Assert.That(elemReq[EElements.EARTH] == 5);
        }

        [Test]
        public void ElemsTest() {
            Assert.That(testReq.Metal() == 5);
            Assert.That(testReq.Wood() == 5);
            Assert.That(testReq.Water() == 5);
            Assert.That(testReq.Fire() == 5);
            Assert.That(testReq.Earth() == 5);
        }

        [Test]
        public void GetReqFromEElements() {
            Assert.That(testReq.GetReqFromEElements(EElements.METAL) == 5);
            Assert.That(testReq.GetReqFromEElements(EElements.WOOD) == 5);
            Assert.That(testReq.GetReqFromEElements(EElements.WATER) == 5);
            Assert.That(testReq.GetReqFromEElements(EElements.FIRE) == 5);
            Assert.That(testReq.GetReqFromEElements(EElements.EARTH) == 5);
        }

        [Test]
        public void SerializeArgumentsTest() {
            JSONArray array = JSON.Parse("[1, 2.0, \"val\"]").AsArray;
            testReq.SerializeArguments(array);
            List<object> args = testReq.RawArguments;
            Assert.That(args.Count == 3);
            Assert.That(args[0].GetType() == typeof(int));
            Assert.That((int) args[0] == 1);
            Assert.That(args[1].GetType() == typeof(double));
            Assert.That((double) args[1] == 2.0);
            Assert.That(args[2].GetType() == typeof(string));
            Assert.That(((string) args[2]).Equals("val"));
        }
	}
}
