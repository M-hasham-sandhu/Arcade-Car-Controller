using UnityEngine;

namespace Car.Modules
{
    public class CarSuspensionModule
    {
        private readonly Rigidbody rb;
        private readonly Transform[] rayPoints;
        private readonly LayerMask driveableLayer;
        private readonly float springStiffness;
        private readonly float damperStiffness;
        private readonly float restLength;
        private readonly float springTravel;
        private readonly float wheelRadius;

        public CarSuspensionModule(
            Rigidbody rb,
            Transform[] rayPoints,
            LayerMask driveableLayer,
            float springStiffness,
            float damperStiffness,
            float restLength,
            float springTravel,
            float wheelRadius)
        {
            this.rb = rb;
            this.rayPoints = rayPoints;
            this.driveableLayer = driveableLayer;
            this.springStiffness = springStiffness;
            this.damperStiffness = damperStiffness;
            this.restLength = restLength;
            this.springTravel = springTravel;
            this.wheelRadius = wheelRadius;
        }

        public void Tick()
        {
            if (rb == null)
            {
                return;
            }

            foreach (Transform rayPoint in rayPoints)
            {
                if (rayPoint == null)
                {
                    continue;
                }

                float maxLength = restLength + springTravel;

                if (Physics.Raycast(rayPoint.position, -rayPoint.up, out RaycastHit hit, maxLength, driveableLayer, QueryTriggerInteraction.Ignore))
                {
                    float currentSpringLength = hit.distance - wheelRadius;
                    float springCompression = Mathf.Clamp01((restLength - currentSpringLength) / springTravel);
                    float springVelocity = Vector3.Dot(rb.GetPointVelocity(rayPoint.position), rayPoint.up);
                    float dampForce = damperStiffness * springVelocity;
                    float springForce = springCompression * springStiffness;
                    float netForce = springForce - dampForce;

                    rb.AddForceAtPosition(rayPoint.up * netForce, rayPoint.position, ForceMode.Force);

                    Debug.DrawLine(rayPoint.position, rayPoint.position + rayPoint.up * hit.distance, Color.red);
                }
                else
                {
                    Debug.DrawLine(rayPoint.position, rayPoint.position - rayPoint.up * (wheelRadius + maxLength), Color.green);
                }
            }
        }
    }
}