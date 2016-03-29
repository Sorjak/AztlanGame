#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PlatformerPro
{
	/// <summary>
	/// A special movement which plays the given animation until completion.
	/// </summary>
	public class SpecialMovement_PlayAnimation : SpecialMovement
	{

		#region members

		/// <summary>
		/// Animation currently being played.
		/// </summary>
		protected AnimationState animationState;

		/// <summary>
		/// Have we started the movement.
		/// </summary>
		protected bool moveStarted;

		/// <summary>
		/// Cached reference to the animator.
		/// </summary>
		protected Animator myAnimator;

		#endregion

		#region constants
		
		/// <summary>
		/// Human readable name.
		/// </summary>
		private const string Name = "Play Animation";
		
		/// <summary>
		/// Human readable description.
		/// </summary>
		private const string Description = "A special movement which plays an animation until completion, typically for mini-cutscenes and interactions.";
		
		/// <summary>
		/// Static movement info used by the editor.
		/// </summary>
		new public static MovementInfo Info
		{
			get
			{
				return new MovementInfo(Name, Description);
			}
		}

		/// <summary>
		/// The size of the movement variable array.
		/// </summary>
		private const int MovementVariableCount = 0;

		#endregion

		#region public methods

		/// <summary>
		/// Initialise this instance.
		/// </summary>
		override public Movement Init(Character character)
		{
			this.character = character;
			myAnimator = GetComponentInChildren<Animator> ();
			if (myAnimator == null) Debug.LogWarning ("Mecanim driven special movement could not find an animator.");
			return this;
		}

		/// <summary>
		/// Initialise the mvoement with the given movement data.
		/// </summary>
		/// <param name="character">Character.</param>
		/// <param name="movementData">Movement data.</param>
		override public Movement Init(Character character, MovementVariable[] movementData)
		{
			this.character = character;

			myAnimator = GetComponentInChildren<Animator> ();
			if (myAnimator == null) Debug.LogWarning ("Mecanim driven special movement could not find an animator.");

			if (movementData != null && movementData.Length >= MovementVariableCount)
			{
			
			}
			else
			{
				Debug.LogError("Invalid movement data.");
			}
			return this;
		}

		/// <summary>
		/// Gets the animation state that this movement wants to set.
		/// </summary>
		override public AnimationState AnimationState
		{
			get 
			{
				if (moveStarted) return animationState;
				return AnimationState.IDLE;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this movement wants to do a special move.
		/// </summary>
		/// <value><c>true</c> if this instance wants control; otherwise, <c>false</c>.</value>
		override public bool WantsSpecialMove()
		{
			return moveStarted;
		}

		/// <summary>
		/// Moves the character.
		/// </summary>
		override public void DoMove()
		{
			// Check if animation is finished
			AnimatorStateInfo info = myAnimator.GetCurrentAnimatorStateInfo(0);
			if (info.IsName(animationState.AsString()))
			{
				if( info.normalizedTime >= 1.0f)
				{
					moveStarted = false;
				}
			}
		}


		/// <summary>
		/// If this is true then the movement wants to maintain control of the character even
		/// if default transition conditions suggest it shouldn't.
		/// </summary>
		/// <returns><c>true</c> if control is wanted, <c>false</c> otherwise.</returns>
		override public bool WantsControl()
		{
			return moveStarted;
		}

		/// <summary>
		/// Start movement with the specified state.
		/// </summary>
		/// <param name="state">Animation state.</param>
		virtual public void Play(AnimationState state)
		{
			moveStarted = true;
			animationState = state;
		}

		/// <summary>
		/// Called when the movement gets control. Typically used to do initialisation of velocity and the like.
		/// </summary>
		override public void GainControl()
		{
			character.SetVelocityX (0);
			character.SetVelocityY (0);
		}


		/// <summary>
		/// Called when the movement loses control. Override to do any reset type actions.
		/// </summary>
		override public void LosingControl()
		{
			moveStarted = false;
			character.SetVelocityX (0);
			character.SetVelocityY (0);
		}

		/// <summary>
		/// Shoulds the apply gravity.
		/// </summary>
		/// <returns><c>true</c>, if apply gravity was shoulded, <c>false</c> otherwise.</returns>
		override public bool ShouldApplyGravity
		{
			get 
			{
				return false;
			}
		}

		/// <summary>
		/// Gets the should do base collisions.
		/// </summary>
		/// <value>The should do base collisions.</value>
		override public RaycastType ShouldDoBaseCollisions
		{
			get
			{
				return RaycastType.NONE;
			}
		}

		#endregion

			
#if UNITY_EDITOR

		#region draw inspector

		/// <summary>
		/// Draws the inspector.
		/// </summary>
		public static MovementVariable[] DrawInspector(MovementVariable[] movementData, ref bool showDetails, Character target)
		{
			if (movementData == null || movementData.Length < MovementVariableCount)
			{
				movementData = new MovementVariable[MovementVariableCount];
			}
			return movementData;
		}

		#endregion

#endif

	}

}

