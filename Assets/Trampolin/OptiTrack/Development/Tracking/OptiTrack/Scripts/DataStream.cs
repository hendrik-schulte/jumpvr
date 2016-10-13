/**
 * Adapted from johny3212
 * Written by Matt Oskamp
 */
using System.Collections.Generic;

namespace OptiTrackManagement
{
    public class DataStream
    {
        private Dictionary<string, int> rbNameToID;

        public int markerCount = 0;
        public int markerIndex = 0;
        public Marker[] marker = new Marker[200];
        public OptiTrackRigidBody[] _rigidBodies = new OptiTrackRigidBody[200];
		public int _nRigidBodies = 0;

		public DataStream ()
		{
			InitializeRigidBody();
            rbNameToID = new Dictionary<string, int>();
		}

        public int GetIDByName(string name)
        {
            int id = -1;

            if(name.Length > 0 && !rbNameToID.TryGetValue(name, out id))
            {
                //Debug.Log("can't match name to id...(no entry in dictionary)");
                foreach (OptiTrackRigidBody rb in _rigidBodies)
                {
                    if(rb.name == name)
                    {
                        rbNameToID.Add(rb.name, rb.ID);
                        return rb.ID;
                    }
                }
                return -1;
            }
            return id;
        }
		
		public bool InitializeRigidBody()
		{
			_nRigidBodies = 0;
			for (int i = 0; i < 200; i++)
			{
				_rigidBodies[i] = new OptiTrackRigidBody();
			}
			return true;
		}

		public OptiTrackRigidBody getRigidbody(int index)
		{
			return _rigidBodies[index];
		}
	}
}