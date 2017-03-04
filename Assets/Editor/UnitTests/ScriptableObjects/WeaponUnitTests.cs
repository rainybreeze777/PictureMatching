using NUnit.Framework;
using NSubstitute;
using System.Collections.Generic;
using UnityEngine;

namespace WeaponUnitTests {

    [TestFixture]
	internal class WeaponUnitTests {

        Weapon testWeapon;
        int testId = 1;
        List<int> testComboIdList = new List<int> {
            1, 2, 3, 4, 5
        };
        string testName = "weapon";
        string testDesc = "description";
        int testTier = 3;
        EElements testElem = EElements.WATER;

        [SetUp]
        public void Init() {
            testWeapon = CreateWeapon(
                            testId
                            , testTier
                            , testName
                            , testDesc
                            , testElem
                            , testComboIdList);
        }

        [Test]
        public void IdTest() {
            Assert.That(testWeapon.ID == testId);
        }

        [Test]
        public void TierTest() {
            Assert.That(testWeapon.Tier == testTier);
        }

        [Test]
        public void ElemTest() {
            Assert.That(testWeapon.Elem == testElem);
        }

        [Test]
        public void GetWeaponNameTest() {
            Assert.That(testWeapon.GetWeaponName().Equals(testName));
        }

        [Test]
        public void GetWeaponDescTest() {
            Assert.That(testWeapon.GetWeaponDesc().Equals(testDesc));
        }

        [Test]
        public void GetWeaponComboIdListTest() {
            List<int> returnedList = testWeapon.GetComboIdList();
            for (int i = 0; i < returnedList.Count; ++i) {
                Assert.That(returnedList[i] == testComboIdList[i]);
            }
        }

        [Test]
        public void WeaponEqualityTest() {
            Weapon otherWeapon = CreateWeapon(
                                    testId
                                    , testTier
                                    , testName
                                    , testDesc
                                    , testElem
                                    , testComboIdList);

            Assert.That(testWeapon.Equals(otherWeapon));
        }

        [Test]
        public void WeaponInequalityTest() {
            Weapon diffIdWeapon = CreateWeapon(
                                    12345
                                    , testTier
                                    , testName
                                    , testDesc
                                    , testElem
                                    , testComboIdList);
            Assert.That(!testWeapon.Equals(diffIdWeapon));
            Weapon diffTierWeapon = CreateWeapon(
                                    testId
                                    , 12345
                                    , testName
                                    , testDesc
                                    , testElem
                                    , testComboIdList);
            Assert.That(!testWeapon.Equals(diffTierWeapon));
            Weapon diffNameWeapon = CreateWeapon(
                                    testId
                                    , testTier
                                    , "someName"
                                    , testDesc
                                    , testElem
                                    , testComboIdList);
            Assert.That(!testWeapon.Equals(diffNameWeapon));
            Weapon diffElemWeapon = CreateWeapon(
                                    testId
                                    , testTier
                                    , testName
                                    , testDesc
                                    , EElements.FIRE
                                    , testComboIdList);
            Assert.That(!testWeapon.Equals(diffElemWeapon));
            Weapon diffComboIdListWeapon = CreateWeapon(
                                    testId
                                    , testTier
                                    , testName
                                    , testDesc
                                    , testElem
                                    , new List<int>{1, 2});
            Assert.That(!testWeapon.Equals(diffComboIdListWeapon));
        }

        private Weapon CreateWeapon(
                int         id
                , int       tier
                , string    name
                , string    desc
                , EElements elem
                , List<int> comboIdList) {

            Weapon newWeapon = ScriptableObject.CreateInstance<Weapon>();
            newWeapon.UnitTesting_SetId(id);
            newWeapon.UnitTesting_SetPossessComboIdList(comboIdList);
            newWeapon.UnitTesting_SetWeaponName(name);
            newWeapon.UnitTesting_SetWeaponDescription(desc);
            newWeapon.UnitTesting_SetTier(tier);
            newWeapon.UnitTesting_SetWeaponElem(elem);

            return newWeapon;
        }
	}
}
