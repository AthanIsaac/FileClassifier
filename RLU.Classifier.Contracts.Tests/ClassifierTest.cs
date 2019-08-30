using System;
using System.IO;
using Autofac.Extras.Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RLU.Classifier.Contracts.Tests
{
    [TestClass]
    public class ClassifierTest
    {
        [TestMethod]
        public void TestGetDisplayNames()
        {
            using (var mock = AutoMock.GetLoose())
            {
                mock.Mock<IDatabaseLayer>()
                    .Setup(x => x.GetDisplayNames(12))
                    .Returns(getNames());

                var m = mock.Create<ClassifierProvider>();
                var e = getNames();
                var actual = m.GetDisplayNamesForTag(12);

                Assert.AreEqual(e.Length, actual.Length);
                
                for (int i = 0; i < e.Length; i++)
                {
                    Assert.AreEqual(e[i], actual[i]);
                }
            }
        }

        private static string[] getNames()
        {
            return new string[] { "this", "worked", "well" };
        }

        [TestMethod]
        public void TestCheckPath()
        {
            using (var mock = AutoMock.GetLoose())
            {
                mock.Mock<IDatabaseLayer>();

                var m = mock.Create<ClassifierProvider>();
                var actual = m.CheckPath("C:");
                Assert.IsTrue(actual);
                actual = m.CheckPath("sdf");
                Assert.IsFalse(actual);
            }
        }

        [TestMethod]
        public void TestGetTags()
        {
            using (var mock = AutoMock.GetLoose())
            {
                mock.Mock<IDatabaseLayer>()
                    .Setup(x => x.GetTags("Success"))
                    .Returns(getTags());

                var m = mock.Create<ClassifierProvider>();
                var actual = m.GetTags("Success");
                var expected = getTags();

                Assert.IsNotNull(actual);
                Assert.AreEqual(actual.Length, expected.Length);
                for (int i = 0; i < actual.Length; i++)
                {
                    Assert.AreEqual(expected[i].name, actual[i].name);
                    Assert.AreEqual(expected[i].id, actual[i].id);
                }

            }
        }
        private static LibraryTag[] getTags()
        {
            return new LibraryTag[] { new LibraryTag(1, "Success") };
        }

        [TestMethod]
        public void TestTagFiles()
        {
            string path = @"C:\Users\copti\Desktop\Church\RLU Project\RLU Test Folder\Hymns\Coptic\Lent\Verses of Cymbals.txt";
            using (var mock = AutoMock.GetLoose())
            {
                mock.Mock<IDatabaseLayer>()
                    .Setup(x => x.TagExists("f"))
                    .Returns(-1);
                mock.Mock<IDatabaseLayer>()
                    .Setup(x => x.AddTag("f"))
                    .Returns(2);
                mock.Mock<IDatabaseLayer>()
                    .Setup(x => x.FileExists(File.ReadAllBytes(path)))
                    .Returns(1);
                mock.Mock<IDatabaseLayer>()
                    .Setup(x => x.AddFileTag(1, 2));

                var m = mock.Create<ClassifierProvider>();
                var actual = m.TagFiles(path, "f");

                Assert.AreEqual(actual, 2);
            }
        }

        [TestMethod]
        public void TestTagFilesTagFound()
        {
            string path = @"C:\Users\copti\Desktop\Church\RLU Project\RLU Test Folder\Hymns\Coptic\Lent\Verses of Cymbals.txt";
            using (var mock = AutoMock.GetLoose())
            {
                mock.Mock<IDatabaseLayer>()
                    .Setup(x => x.TagExists("t"))
                    .Returns(14);
                mock.Mock<IDatabaseLayer>()
                    .Setup(x => x.FileExists(File.ReadAllBytes(path)))
                    .Returns(1);
                mock.Mock<IDatabaseLayer>()
                    .Setup(x => x.AddFileTag(1, 14));

                var m = mock.Create<ClassifierProvider>();
                var actual = m.TagFiles(path, "t");

                Assert.AreEqual(14, actual);
            }
        }

        [TestMethod]
        public void TestTagFilesThrows()
        {
            string path = @"C:\Users\copti\Desktop\Church\RLU Project\RLU Test Folder\Hymns\Coptic\Kiahk\Doxology.txt";
            using (var mock = AutoMock.GetLoose())
            {
                mock.Mock<IDatabaseLayer>()
                    .Setup(x => x.TagExists("t"))
                    .Returns(14);
                mock.Mock<IDatabaseLayer>()
                    .Setup(x => x.FileExists(File.ReadAllBytes(path)))
                    .Returns(-1);
                mock.Mock<IDatabaseLayer>()
                    .Setup(x => x.AddFileTag(1, 14));


                var m = mock.Create<ClassifierProvider>();
                try
                {
                    var actual = m.TagFiles(path, "t");
                }
                catch(IOException e)
                {
                    Assert.AreEqual(e.Message, "This directory has no non-empty files", "wrong exception thrown");
                    return;
                }
                Assert.IsTrue(false, "did not throw exception");
            }
        }

        [TestMethod]
        public void TestTagFileNotInDB()
        {
            string path = @"C:\Users\copti\Desktop\Church\RLU Project\RLU Test Folder\Hymns\Coptic\Lent\Verses of Cymbals.txt";
            using (var mock = AutoMock.GetLoose())
            {
                mock.Mock<IDatabaseLayer>()
                    .Setup(x => x.TagExists("t"))
                    .Returns(14);
                mock.Mock<IDatabaseLayer>()
                    .Setup(x => x.FileExists(File.ReadAllBytes(path)))
                    .Returns(-1);
                mock.Mock<IDatabaseLayer>()
                    .Setup(x => x.AddFile(File.ReadAllBytes(path), "Verses of Cymbals", ".txt"))
                    .Returns(new LibraryFile(1, "Verses of Cymbals", ".txt"));
                mock.Mock<IDatabaseLayer>()
                    .Setup(x => x.AddFileTag(1, 14));


                var m = mock.Create<ClassifierProvider>();
                
                    var actual = m.TagFiles(path, "t");

                Assert.AreEqual(14, actual);
            }
        }

        [TestMethod]
        public void TestAddDisplayNamesToTag()
        {
            using (var mock = AutoMock.GetLoose())
            {
                mock.Mock<IDatabaseLayer>()
                    .Setup(x => x.AddDisplayNameToTag("t", 3));
                mock.Mock<IDatabaseLayer>()
                    .Setup(x => x.AddDisplayNameToTag("f", 3));

                var m = mock.Create<ClassifierProvider>();

                m.AddDisplayNamesToTag(3, "t\nf");
                Assert.IsFalse(false);
            }
        }
        [TestMethod]
        public void TestGetDisplayNamesForTag()
        {
            using (var mock = AutoMock.GetLoose())
            {
                mock.Mock<IDatabaseLayer>()
                    .Setup(x => x.GetDisplayNames(3))
                    .Returns(getNames());

                var m = mock.Create<ClassifierProvider>();

                var actual = m.GetDisplayNamesForTag(3);

                var e = getNames();
                Assert.AreEqual(e.Length, actual.Length);

                for (int i = 0; i < e.Length; i++)
                {
                    Assert.AreEqual(e[i], actual[i]);
                }

            }
        }
    }
}
