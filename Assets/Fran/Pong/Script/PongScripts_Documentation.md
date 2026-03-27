# Pong Scripts - Technical Documentation

## Scope
This document covers the runtime behavior, dependencies, and maintenance risks of:

- `BombMovement.cs`
- `PongMovement.cs`
- `PongRaycast.cs`
- `TestRotacion.cs`

---

## `BombMovement`

### Purpose
Controls the bomb/ball movement in the Pong mode:

- Sets initial linear velocity.
- Reflects direction on collisions.
- Increases speed by a percentage on each impact.
- Clamps velocity to a configured maximum speed.

### Runtime Flow
1. `Start()`
   - Initializes `currentSpeed` from `speed`.
   - Converts percentage to per-hit multiplier component (`speedIncreasePercentage / 100f`).
   - Gets `Rigidbody2D`.
   - Starts moving to `transform.right`.
2. `OnCollisionEnter2D()`
   - Reads first contact normal.
   - Reflects current velocity using `Vector2.Reflect`.
   - Applies speed growth and clamp.
   - Writes final velocity back to `Rigidbody2D`.
   - Stores normalized direction.

### Serialized Tuning Parameters
- `speed`: initial speed.
- `speedIncreasePercentage`: speed growth percent per collision.
- `maxSpeed`: hard speed cap.

### Key Risks / Future Failures
- **Inspector misconfiguration risk**:
  - `maxSpeed <= 0` can stop movement after first collision.
  - `speed <= 0` starts with no movement.
  - very high `speedIncreasePercentage` can cause unstable feel.
- **Debug spam**:
  - `Debug.Log` calls in collision paths can flood logs in long sessions.
- **Dead fields and debug leftovers**:
  - `angle`, `previousDirection`, `collision`, and `OnCollisionStay2D` currently do not affect gameplay.

### Recommended Hardening
- Validate config in `Start()`:
  - enforce `speed > 0`.
  - enforce `maxSpeed >= speed`.
  - enforce `speedIncreasePercentage >= 0`.
- Remove or gate logs behind a debug flag.
- Reduce public mutable state when not needed by other systems.

---

## `PongMovement`

### Purpose
Handles player paddle/element rotation from Input System callbacks.

### Runtime Flow
1. `Start()`
   - Caches `PlayerInput` and `CharacterController`.
2. `OnMove(InputAction.CallbackContext callback)`
   - Reads input vector.
   - On `Performed`, rotates object on Z axis using `direction.x`.

### Dependencies
- `UnityEngine.InputSystem.PlayerInput`
- Input action callback wiring (expected from `PlayerInput` events)

### Key Risks / Future Failures
- **Potential non-continuous movement**:
  - Rotating only on `Performed` depends on action behavior. If callback is not repeatedly fired while held, rotation may feel inconsistent.
- **Unused component dependency**:
  - `CharacterController controller` is fetched but not used.
- **Debug spam**:
  - `Debug.Log("rotando")` and input value logs run during gameplay input.

### Recommended Hardening
- If continuous rotation is desired, cache input value and apply rotation in `Update()`.
- Remove unused `CharacterController` if not required.
- Keep input logs only in debug sessions.

---

## `PongRaycast`

### Purpose
Per-frame raycast utility/debug visual from object transform.

### Runtime Flow
1. `Start()`
   - Sets static singleton reference (`instance = this`).
2. `Update()`
   - Performs downward raycast and stores result in `hit`.
   - Performs right-direction raycast (result currently ignored).
   - Draws debug ray in editor/game view.

### Key Risks / Future Failures
- **Singleton overwrite risk**:
  - If multiple objects have this component, `instance` will be replaced by the last initialized one.
- **Unused raycast result**:
  - second raycast call return value is discarded.
- **Uncontrolled public mutable fields**:
  - `hit` and `ray` are public and can be modified externally.

### Recommended Hardening
- Add singleton guard if global access is required.
- Store or remove unused raycast call.
- Make fields private unless external systems really need direct access.

---

## `TestRotacion`

### Purpose
Editor visualization helper that draws an arc around the object.

### Runtime Flow
- Uses `OnDrawGizmos()` and `Handles.DrawWireArc` to render an arc with configurable `angle`.

### Critical Risk / Future Failure
- **Build break risk (high)**:
  - Script references `UnityEditor` in a normal runtime folder.
  - `UnityEditor` APIs are editor-only and can fail player builds unless protected.

### Recommended Hardening
- Wrap editor-only code:
  - `#if UNITY_EDITOR` around `using UnityEditor` and `OnDrawGizmos` implementation.
- Or move this script into an `Editor` folder if it should never be runtime-compiled.

---

## Production Readiness Checklist

- [ ] Remove or guard all non-essential `Debug.Log` in gameplay scripts.
- [ ] Add serialized value validation in `BombMovement`.
- [ ] Resolve `UnityEditor` dependency exposure in `TestRotacion`.
- [ ] Remove unused variables/methods and stale comments.
- [ ] Decide if `PongMovement` should be event-driven or continuous-update driven.

---

## Suggested Test Matrix

1. **Bomb baseline**
   - `speed=6`, `speedIncreasePercentage=0`, `maxSpeed=20`.
   - Expected: stable constant speed after all rebounds.
2. **Bomb scaling**
   - `speed=6`, `speedIncreasePercentage=10`, `maxSpeed=20`.
   - Expected: speed increases per collision, then caps at 20.
3. **Bomb invalid values**
   - `maxSpeed=0`, `speed=6`.
   - Expected: validation warning and auto-correction (after hardening).
4. **Input hold behavior**
   - Hold move key/stick for 3 seconds.
   - Expected: consistent paddle rotation with no jitter or dead input windows.
5. **Build test**
   - Build Windows player.
   - Expected: no `UnityEditor` compile issues after `TestRotacion` hardening.
