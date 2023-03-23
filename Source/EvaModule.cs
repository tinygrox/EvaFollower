namespace MSD.EvaFollower
{
    /// <summary>
    /// Keep track of the Context Menu.
    /// </summary>
    class EvaModule : PartModule
    {
        private EvaContainer currentContainer;

        public void Update()
        {
            if (!FlightGlobals.ready || PauseMenu.isOpen)
                return;

            if (currentContainer == null)
                return;

                ResetEvents();
                SetEvents();

        }

        public void Load(EvaContainer current)
        {
            this.currentContainer = current;
        }

        /// <summary>
        /// The default events based on the kerbal status.
        /// </summary>
        public void ResetEvents()
        {
            Events["Follow"].active = false;
            Events["Stay"].active = false;
            Events["SetPoint"].active = false;
            Events["Wait"].active = false;
            Events["Patrol"].active = false;
            Events["EndPatrol"].active = false;
            Events["PatrolRun"].active = false;
            Events["PatrolWalk"].active = false;
			Events["StartWanderer"].active = false;
        }

        /// <summary>
        /// Set events based on the kerbal status.
        /// </summary>
        public void SetEvents()
        {
            if (!currentContainer.Loaded)
                return;

			if (!currentContainer.EVA.vessel.Landed && !currentContainer.EVA.vessel.Splashed) {
				return; 
			}

            if (currentContainer.mode == Mode.None)
            {
                Events["Follow"].active = true;
                Events["Stay"].active = false;
				//Events["StartWanderer"].active = true;
            }
            else if (currentContainer.mode == Mode.Follow)
            {
                Events["Follow"].active = false;
                Events["Stay"].active = true;
            }
			else if (currentContainer.mode == Mode.Patrol)
            {
                if (currentContainer.AllowRunning)
                {
                    Events["PatrolWalk"].active = true;
                }
                else
                {
                    Events["PatrolRun"].active = true;
                }

                Events["Patrol"].active = false;
                Events["EndPatrol"].active = true;
            }
            else if (currentContainer.mode == Mode.Order)
            {
                Events["Stay"].active = true;
                Events["Follow"].active = true;
            }

            if (currentContainer.IsActive)
            {
                Events["Follow"].active = false;
                Events["Stay"].active = false;
                Events["SetPoint"].active = true;
                Events["Wait"].active = true;

                if (currentContainer.mode != Mode.Patrol)
                {
                    if (currentContainer.AllowPatrol)
                    {
                        Events["Patrol"].active = true;
                    }
                }
                else
                {
                    Events["SetPoint"].active = false;
                    Events["Wait"].active = false;
                }
            }
        }


        [KSPEvent(guiActive = true, guiName = "跟着我", active = true, guiActiveUnfocused = true, unfocusedRange = 8)] // "Follow Me"
        public void Follow()
        {
            currentContainer.Follow();
        }

        [KSPEvent(guiActive = true, guiName = "待着别动", active = true, guiActiveUnfocused = true, unfocusedRange = 8)] // "Stay Put"
        public void Stay()
        {
            currentContainer.Stay();
        }

        [KSPEvent(guiActive = true, guiName = "设置导航点", active = true, guiActiveUnfocused = true, unfocusedRange = 8)] // "Add Waypoint"
        public void SetPoint()
        {
                currentContainer.SetWaypoint();
        }

        [KSPEvent(guiActive = true, guiName = "等待", active = true, guiActiveUnfocused = true, unfocusedRange = 8)] // "Wait"
        public void Wait()
        {
            currentContainer.Wait();
        }

        [KSPEvent(guiActive = true, guiName = "巡逻", active = true, guiActiveUnfocused = true, unfocusedRange = 8)] // "Patrol"
        public void Patrol()
        {
            currentContainer.StartPatrol();
        }

        [KSPEvent(guiActive = true, guiName = "结束巡逻", active = true, guiActiveUnfocused = true, unfocusedRange = 8)] // "End Patrol"
        public void EndPatrol()
        {
            currentContainer.EndPatrol();
        }

        [KSPEvent(guiActive = true, guiName = "行走", active = true, guiActiveUnfocused = true, unfocusedRange = 8)] //"Walk"
        public void PatrolWalk()
        {
            currentContainer.SetWalkPatrolMode();
        }

        [KSPEvent(guiActive = true, guiName = "奔跑", active = true, guiActiveUnfocused = true, unfocusedRange = 8)] // "Run"
        public void PatrolRun()
        {
            currentContainer.SetRunPatrolMode();
        }

		[KSPEvent(guiActive = true, guiName = "闲逛", active = true, guiActiveUnfocused = true, unfocusedRange = 8)] // "Wander"
		public void StartWanderer()
		{
			currentContainer.StartWanderer();
		}

        /*
        [KSPEvent(guiActive = true, guiName = "Debug", active = true, guiActiveUnfocused = true, unfocusedRange = 8)]
        public void Debug()
        {
            foreach (var item in EvaController.fetch.collection)
            {
                EvaDebug.DebugLog("Item: " + item.flightID);
                EvaDebug.DebugLog("leader: " + item.formation.GetLeader());
                EvaDebug.DebugLog("patrol: " + item.patrol.ToString());
                EvaDebug.DebugLog("order: " + item.order.ToString());
                EvaDebug.DebugLog("patrol: " + item.patrol);
            }

            currentContainer.EVA.headLamp.light.intensity += 100;
        }


        [KSPEvent(guiActive = true, guiName = "Save", active = true, guiActiveUnfocused = true, unfocusedRange = 8)]
        public void ClearSave()
        {
            EvaSettings.SaveFunction();
        }


        [KSPEvent(guiActive = true, guiName = "Load", active = true, guiActiveUnfocused = true, unfocusedRange = 8)]
        public void Load()
        {
            EvaSettings.LoadFunction();
        }
        */

    }
}
