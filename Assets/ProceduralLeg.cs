using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralLeg : MonoBehaviour
{
    //float thigh_length = 12;
    //float calf_length = 12;

    // right leg
    float r_move = 0;

    float r_hip_x;
    float r_hip_y;
    float r_knee_x;
    float r_knee_y;
    float r_foot_x;
    float r_foot_y;

    // left leg
    float l_move = 90;

    float l_hip_x;
    float l_hip_y;
    float l_knee_x;
    float l_knee_y;
    float l_foot_x;
    float l_foot_y;

    float speed = 0;

    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float hor = Input.GetAxis("Horizontal");
        float ver = Input.GetAxis("Vertical");

        if(speed > 0.1f)
        {
            var movespd = 2;
            r_move += movespd;
            l_move += movespd;

            //right leg
            //knee
        }
    }
}
