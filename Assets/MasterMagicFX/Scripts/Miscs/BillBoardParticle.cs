using UnityEngine;
using System.Collections;
namespace MasterFX
{
    [ExecuteInEditMode]
    public class BillBoardParticles : MonoBehaviour
    {

        public bool bTurnOver = false;

        void OnWillRenderObject()
        {
            if (Camera.current)
            {
                if (bTurnOver)
                    transform.forward = Camera.current.transform.forward;
                else
                    transform.forward = -Camera.current.transform.forward;
            }
        }
    }
}