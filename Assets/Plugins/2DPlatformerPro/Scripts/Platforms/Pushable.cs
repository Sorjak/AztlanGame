using UnityEngine;
using System.Collections;

namespace PlatformerPro
{
	/// <summary>
	/// An object that can be pushed.
	/// </summary>
	[RequireComponent (typeof(Rigidbody2D))]
	public class Pushable : MonoBehaviour
	{
	
		[SerializeField]
		[Tooltip ("Should we reset the velocity to zero before pushing.")]
		protected bool resetVelocityOnPush = true;

		/// <summary>
		/// When using move position we use this variable to help us simulate gravity.
		/// </summary>
		protected float expectedY;

		/// <summary>
		/// Do we have a target position
		/// </summary>
		protected bool targetSet = false;

		/// <summary>
		/// The target position for a MovePosition move.
		/// </summary>
		protected Vector2 targetPosition;

		/// <summary>
		/// Do we have a force to add.
		/// </summary>
		protected bool forceSet = false;
		
		/// <summary>
		/// The force to apply when using force based pushing.
		/// </summary>
		protected Vector2 force;

		/// <summary>
		/// Original mass, we store it here as we change it while pushing if push mode is MovePosition().
		/// </summary>
		protected float mass;

		/// <summary>
		/// Has FixedUpdate been executed?
		/// </summary>
		protected bool physicsUpdatedThisFrame;

		/// <summary>
		/// Reference to the rigidbody 2D.
		/// </summary>
		new protected Rigidbody2D rigidbody2D;

		/// <summary>
		/// Get the pushables weight.
		/// </summary>
		/// <value>The weight.</value>
		virtual public float Mass
		{
			get { return mass; }
		}

		/// <summary>
		/// Unity Start() hook.
		/// </summary>
		void Start()
		{
			rigidbody2D = GetComponent<Rigidbody2D> ();
			if (rigidbody2D == null) Debug.LogWarning ("A Pushable requires a Rigidbody2D");
			mass = rigidbody2D.mass;
		}

		/// <summary>
		/// Push this pushable.
		/// </summary>
		/// <param name="character">Character who is pushing..</param>
		/// <param name="amount">Amount to push.</param>
		virtual public void Push(IMob character, Vector2 amount, bool pushAsForce)
		{
			if (resetVelocityOnPush) rigidbody2D.velocity = Vector2.zero;
			if (pushAsForce)
			{
				force = amount;
				forceSet = true;
			}
			else
			{
				targetPosition = amount;
				targetSet = true;
			}
		}

		/// <summary>
		/// LateUpdate hook.
		/// </summary>
		void LateUpdate()
		{
			// If physics weren't updated this frame we need to move the box anyway else it will slow us down as frame rate increases
			if (!physicsUpdatedThisFrame && targetSet)
			{
				transform.Translate(targetPosition);
			}
			else
			{
				// For some reason transform position and rigidboy position get out of synch using Move Position. Here we fix it.
				transform.position = new Vector3(rigidbody2D.position.x, rigidbody2D.position.y, transform.position.z);
			}
			physicsUpdatedThisFrame = false;
		}

		/// <summary>
		/// Unity Fixed Update, does most of the work.
		/// </summary>
		void FixedUpdate()
		{
			physicsUpdatedThisFrame = true;
			if (targetSet) {
				rigidbody2D.mass = 1;
				// Simulate a simple gravity like force so that we fall even though we are using MovePosition()
				if (expectedY == rigidbody2D.position.y )
			    {
					rigidbody2D.velocity += new Vector2(0, Physics2D.gravity.y * Time.fixedDeltaTime);
					targetPosition += new Vector2(0, rigidbody2D.velocity.y * Time.fixedDeltaTime);
				}
				else 
				{
					rigidbody2D.velocity = new Vector3(rigidbody2D.velocity.x, 0);
				}
				expectedY = targetPosition.y + rigidbody2D.position.y;

				// Move to position
				rigidbody2D.MovePosition ((Vector2)rigidbody2D.position + targetPosition);

				targetSet = false;
			}
			else
			{
				rigidbody2D.mass = mass;
			}

			if (forceSet)
			{
				rigidbody2D.AddForce (force, ForceMode2D.Force);
				forceSet = false;
			}
		}


	}
}
