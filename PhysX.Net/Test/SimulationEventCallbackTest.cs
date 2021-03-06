﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PhysX.Test
{
	[TestClass]
	public class SimulationEventCallbackTest : Test
	{
		[TestMethod]
		[Ignore]
		public void OnConstraintBreakIsCalled()
		{
			using (var physics = CreatePhysicsAndScene())
			{
				var callback = new MockSimulationEventCallback();

				physics.Scene.SetSimulationEventCallback(callback);

				var box = CreateBoxActor(physics.Scene, new Vector3(5, 5, 5), new Vector3(0, 10, 0));

				var joint = physics.Scene.CreateJoint<SphericalJoint>(box, Matrix4x4.Identity, null, Matrix4x4.CreateTranslation(0, 50, 0));
				joint.BreakForce = 0.001f;
				//joint.ConstraintFlag = ConstraintFlag.Reporting;

				for (int i = 0; i < 200; i++)
				{
					physics.Scene.Simulate(0.2f);
					physics.Scene.FetchResults(block: true);
				}

				Assert.IsTrue(callback.BrokenJoints.Contains(joint));
			}
		}

		private class MockSimulationEventCallback : SimulationEventCallback
		{
			public List<PhysX.Joint> BrokenJoints { get; set; }

			public MockSimulationEventCallback()
			{
				BrokenJoints = new List<PhysX.Joint>();
			}

			public override void OnConstraintBreak(ConstraintInfo[] constraints)
			{
				var joint = constraints[0].ExternalReference as PhysX.Joint;

				BrokenJoints.Add(joint);
			}
		}
	}
}