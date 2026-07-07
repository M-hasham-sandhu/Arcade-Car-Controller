using UnityEngine;

namespace Car.Modules
{
    public class CarMotorModule
    {
        private readonly Rigidbody rb;
        private readonly Transform carTransform;
        private readonly float maxMotorForce;
        private readonly float brakeForce;
        private readonly float reverseMultiplier;
        private readonly float accelerationMultiplier;
        private readonly float decelerationMultiplier;
        private readonly float handbrakeForce;
        private readonly float rollingResistance;
        private readonly Transform[] rearWheelTransforms;

        public CarMotorModule(
            Rigidbody rb,
            Transform carTransform,
            float maxMotorForce,
            float brakeForce,
            float reverseMultiplier = 0.6f,
            float accelerationMultiplier = 1f,
            float decelerationMultiplier = 0.8f,
            float handbrakeForce = 1200f,
            float rollingResistance = 0.08f,
            Transform[] rearWheelTransforms = null)
        {
            this.rb = rb;
            this.carTransform = carTransform;
            this.maxMotorForce = maxMotorForce;
            this.brakeForce = brakeForce;
            this.reverseMultiplier = reverseMultiplier;
            this.accelerationMultiplier = accelerationMultiplier;
            this.decelerationMultiplier = decelerationMultiplier;
            this.handbrakeForce = handbrakeForce;
            this.rollingResistance = rollingResistance;
            this.rearWheelTransforms = rearWheelTransforms;
        }

        /// <summary>
        /// Drive, brake, and rolling-resistance forces are applied purely along the car's
        /// current forward axis. They automatically track the new heading as the car turns,
        /// and they never touch the sideways (right) axis - that's handled exclusively by
        /// CarSteeringModule's grip correction, so the two systems never fight each other.
        /// </summary>
        public void SetDriveInput(float throttleInput)
        {
            if (rb == null || carTransform == null)
            {
                return;
            }

            Vector3 forwardDirection = carTransform.forward;
            Vector3 currentVelocity = rb.linearVelocity;
            float forwardSpeed = Vector3.Dot(currentVelocity, forwardDirection);

            if (Mathf.Abs(forwardSpeed) > 0.01f)
            {
                float resistanceSign = Mathf.Sign(forwardSpeed);
                rb.AddForce(-forwardDirection * (resistanceSign * rollingResistance), ForceMode.Acceleration);
            }

            if (throttleInput > 0f)
            {
                float motorForce = maxMotorForce * throttleInput * accelerationMultiplier;
                rb.AddForce(forwardDirection * motorForce, ForceMode.Force);
            }
            else if (throttleInput < 0f)
            {
                float motorForce = maxMotorForce * reverseMultiplier * Mathf.Abs(throttleInput) * accelerationMultiplier;
                rb.AddForce(-forwardDirection * motorForce, ForceMode.Force);
            }
            else if (Mathf.Abs(forwardSpeed) > 0.05f)
            {
                Vector3 brakingDirection = -forwardDirection * Mathf.Sign(forwardSpeed);
                rb.AddForce(brakingDirection * (brakeForce * decelerationMultiplier), ForceMode.Force);
            }
            else
            {
                // Kill any tiny residual forward creep so the car fully settles at rest.
                rb.linearVelocity -= forwardDirection * forwardSpeed;
            }
        }

        public void ApplyBraking(float brakeInput)
        {
            if (rb == null || carTransform == null)
            {
                return;
            }

            Vector3 forwardDirection = carTransform.forward;
            Vector3 currentVelocity = rb.linearVelocity;
            float forwardSpeed = Vector3.Dot(currentVelocity, forwardDirection);
            Vector3 brakingDirection = -forwardDirection * Mathf.Sign(forwardSpeed);
            Vector3 brakingForce = brakingDirection * (brakeForce * brakeInput);

            rb.AddForce(brakingForce, ForceMode.Force);
            rb.AddTorque(-rb.angularVelocity * (0.15f * brakeInput), ForceMode.Acceleration);
        }

        public void ApplyHandbrake(bool isHandbrakeActive)
        {
            if (rb == null || carTransform == null || !isHandbrakeActive)
            {
                return;
            }

            Vector3 rearwardDirection = -carTransform.forward;
            Vector3 lateralVelocity = Vector3.Project(rb.linearVelocity, carTransform.right);

            // The handbrake intentionally breaks rear grip, so unlike normal driving
            // it's allowed to leave some sideways velocity (i.e. a controlled slide).
            rb.AddForce(lateralVelocity * -0.35f, ForceMode.Acceleration);

            if (rearWheelTransforms != null)
            {
                foreach (Transform rearWheelTransform in rearWheelTransforms)
                {
                    if (rearWheelTransform == null)
                    {
                        continue;
                    }

                    rb.AddForceAtPosition(rearwardDirection * handbrakeForce, rearWheelTransform.position, ForceMode.Force);
                }
            }
            else
            {
                rb.AddForceAtPosition(rearwardDirection * handbrakeForce, carTransform.position - carTransform.forward * 0.8f, ForceMode.Force);
            }
        }
    }
}