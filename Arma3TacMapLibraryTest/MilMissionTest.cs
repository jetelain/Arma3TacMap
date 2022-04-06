using System;
using System.Collections.Generic;
using System.Text;
using Arma3TacMapLibrary.Maps;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Arma3TacMapLibraryTest
{
    [TestClass]
    public class MilMissionTest
    {
        [TestMethod]
        public void MilMission_ToSeize()
        {
            var worker = new MilMission();

            var result = worker.RenderMission("toSeize", new[] { new[] { 1000d, 1000d }, new[] { 1000d, 3000d } }, 50);

        }

    }
}
