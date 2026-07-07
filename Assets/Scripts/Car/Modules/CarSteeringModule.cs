using UnityEngine;

namespace Car.Modules
{
    public class CarSteeringModule
    {
        private readonly Rigidbody rb;
        private readonly Transform carTransform;
        private readonly Transform[] wheelTransforms;
        private readonly float maxSteerAngle;
        private readonly float steerSpeed;
        private readonly float turnTorque;
        private readonly float gripMultiplier;
        private readonly float handbrakeGripMultiplier;
        private readonly float minSteeringSpeed;
        private readonly float steeringDamping;

        private float currentSteerAngle;

        public CarSteeringModule(
            Rigidbody rb,
            Transform carTransform,
            Transform[] wheelTransforms,
            float maxSteerAngle,
            float steerSpeed,
            float turnTorque,
            float gripMultiplier,
            float handbrakeGripMultiplier,
            float minSteeringSpeed,
            float steeringDamping)
        {
            this.rb = rb;
            this.carTransform = carTransform;
            this.wheelTransforms = wheelTransforms;
            this.maxSteerAngle = maxSteerAngle;
            this.steerSpeed = steerSpeed;
            this.turnTorque = turnTorque;
            this.gripMultiplier = gripMultiplier;
            this.handbrakeGripMultiplier = handbrakeGripMultiplier;
            this.minSteeringSpeed = minSteeringSpeed;
            this.steeringDamping = steeringDamping;
        }

        public void SetSteerInput(float steerInput, bool isHandbrakeActive = false)
        {
            if (rb == null)
            {
                return;
            }

            float targetAngle = Mathf.Clamp(steerInput, -1f, 1f) * maxSteerAngle;
            currentSteerAngle = Mathf.Lerp(currentSteerAngle, targetAngle, steerSpeed * Time.fixedDeltaTime);

            ApplySteerVisual();
            ApplyGrip(isHandbrakeActive);
            ApplyTurning(steerInput);
        }

        private void ApplySteerVisual()
        {
            foreach (Transform wheelTransform in wheelTransforms)
            {
                if (wheelTransform == null)
                {
                    continue;
                }

                wheelTransform.localRotation = Quaternion.Euler(0f, currentSteerAngle, 0f);
            }
        }

        /// <summary>
        /// Cancels the car's sideways velocity every physics step. This is what makes the
        /// car's actual motion realign with its forward direction as it turns, instead of
        /// carrying old momentum sideways (i.e. no drift/slide). gripMultiplier is the
        /// fraction of sideways velocity removed per FixedUpdate - close to 1 gives a
        /// glued-to-the-road arcade feel, lower values allow progressively more slide.
        /// </summary>
        private void ApplyGrip(bool isHandbrakeActive)
        {
            if (carTransform == null)
            {
                return;
            }

            Vector3 currentVelocity = rb.linearVelocity;
            Vector3 lateralVelocity = Vector3.Project(currentVelocity, carTransform.right);
            float grip = Mathf.Clamp01(isHandbrakeActive ? handbrakeGripMultiplier : gripMultiplier);

            rb.AddForce(-lateralVelocity * grip, ForceMode.VelocityChange);
        }

        /// <summary>
        /// Rotates the car toward the steering input. Because ApplyGrip() re-aligns velocity
        /// to forward every step, rotating the car effectively rotates the direction of travel
        /// too - there's no separate lateral "push" force here that could fight that alignment.
        /// </summary>
        private void ApplyTurning(float steerInput)
        {
            if (carTransform == null)
            {
                return;
            }

            float steeringInput = Mathf.Clamp(steerInput, -1f, 1f);
            Vector3 currentVelocity = rb.linearVelocity;
            float speed = currentVelocity.magnitude;

            if (speed < minSteeringSpeed)
            {
                Vector3 lowSpeedYaw = Vector3.Project(rb.angularVelocity, carTransform.up);
                rb.AddTorque(-lowSpeedYaw * steeringDamping, ForceMode.Acceleration);
                return;
            }

            float forwardSpeed = Vector3.Dot(currentVelocity, carTransform.forward);
            float speedFactor = Mathf.Clamp01(Mathf.Abs(forwardSpeed) / 12f);
            float direction = forwardSpeed >= 0f ? 1f : -1f; // flip so steering feels correct in reverse
            float steeringTorque = steeringInput * turnTorque * speedFactor * direction;

            rb.AddTorque(carTransform.up * steeringTorque, ForceMode.Acceleration);

            Vector3 yawVelocity = Vector3.Project(rb.angularVelocity, carTransform.up);
            rb.AddTorque(-yawVelocity * steeringDamping, ForceMode.Acceleration);
        }
    }
}