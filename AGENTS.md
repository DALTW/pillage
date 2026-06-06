# Repository Guidelines

## Project Structure & Module Organization
This is a Unity project targeting editor version `6000.3.11f1`. Game code and authored assets live under `Assets/`. Key areas include `Assets/Scenes/` for scenes, `Assets/camera/` for camera scripts, `Assets/Player/` for player-related assets, and `Assets/map asset/` for sketchbook map gameplay, prefabs, and scripts. Project configuration is in `ProjectSettings/`; package dependencies are in `Packages/manifest.json`.

Do not commit generated Unity folders such as `Library/`, `Temp/`, `Obj/`, `Logs/`, `UserSettings/`, or `Assets/_Recovery/`.

## Build, Test, and Development Commands
Open the project with Unity Hub using Unity `6000.3.11f1`, or launch the editor with:

```powershell
"C:\Program Files\Unity\Hub\Editor\6000.3.11f1\Editor\Unity.exe" -projectPath .
```

Run EditMode tests in batch mode:

```powershell
"C:\Program Files\Unity\Hub\Editor\6000.3.11f1\Editor\Unity.exe" -batchmode -projectPath . -runTests -testPlatform EditMode -testResults TestResults.xml -quit
```

Run PlayMode tests by changing `-testPlatform PlayMode`. There is no repository-specific build script; create builds from Unity Build Profiles/Build Settings unless a CI build method is added.

## Coding Style & Naming Conventions
Use C# conventions common to Unity: PascalCase for classes, methods, properties, and public enum values; camelCase for private fields and local variables. Keep `MonoBehaviour` scripts focused on one component responsibility. Prefer serialized fields or public fields only when Unity Inspector editing is required. Keep `.meta` files with their assets and scripts.

## Testing Guidelines
The Unity Test Framework package is installed, but no test folders are currently present. Add EditMode tests under `Assets/Tests/EditMode/` and PlayMode tests under `Assets/Tests/PlayMode/`. Name test classes after the behavior under test, for example `CameraFollow2DTests`, and keep test names descriptive.

## Commit & Pull Request Guidelines
Recent history uses a mix of concise conventional-style commits, such as `feat(map): ...` and `fix(camera): ...`, plus direct update messages. Prefer `feat(scope): summary`, `fix(scope): summary`, or `docs: summary` for new work.

For pull requests, include a short description, changed scenes/assets, test results or manual Unity validation, and screenshots or clips for visible gameplay/UI changes. Mention any generated assets, prefab changes, or scene edits explicitly.
