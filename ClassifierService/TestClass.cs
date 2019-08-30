using Autofac.Extras.Moq;
using RLU.Classifier.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ClassifierService
{
    class TestClass
    {
        static void Main(string[] args)
        {
            test1();
            Console.ReadLine();  
        }
        public static void test1()
        {
            using (var mock = AutoMock.GetLoose())
            {
                mock.Mock<IDatabaseLayer>()
                    .Setup(x => x.GetDisplayNames(12))
                    .Returns(getNames());

                var m = mock.Create<ClassifierProvider>();
                var e = getNames();
                var actual = m.GetDisplayNamesForTag(12);  
            }
        }

        private static string[] getNames()
        {
            return new string[] { "this" , "worked" };
        }
    }
}
