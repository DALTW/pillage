---
title: "Pencil Village - Technical Spec"
subtitle: "기술 구현 명세 문서"
date: "2026-05-15"
lang: ko-KR
mainfont: "Noto Sans CJK KR"
monofont: "NanumGothicCoding"
fontsize: 10.5pt
geometry:
  - margin=20mm
toc: true
toc-depth: 2
numbersections: true
---

# 문서 목적

이 문서는 Pencil Village를 Vite + TypeScript + Canvas API 기반으로 구현하기 위한 기술 명세다. AI 코딩 도구 또는 개발자가 바로 구현을 시작할 수 있도록 주요 시스템, 상태, 파일 구조, 데이터 모델을 정의한다.

# 기술 스택

- Vite
- TypeScript
- Canvas API
- 브라우저 싱글플레이
- 저장 기능 없음
- 서버 없음
- 로그인 없음

# 개발 목표

MVP는 처음부터 엔딩까지 10분 안에 플레이 가능한 브라우저 게임이어야 한다.

필수 구현:

- 도입 연출
- 오두막 내부
- 외부 필드
- 플레이어 이동
- 카메라 추적
- 상호작용 시스템
- NPC 아이콘 퀘스트
- 연필 조각 시스템
- 크레파스 시스템
- 스케치북 UI
- 숲 확장 3단계
- 최종 문과 엔딩

# 프로젝트 구조 예시

```text
src/
  main.ts
  game/
Game.ts
SceneManager.ts
InputManager.ts
Camera.ts
Time.ts
  scenes/
IntroScene.ts
CabinScene.ts
WorldScene.ts
SketchbookScene.ts
EndingScene.ts
  entities/
Entity.ts
Player.ts
NPC.ts
Interactable.ts
Collectible.ts
Door.ts
  world/
WorldMap.ts
CollisionMap.ts
ExpansionState.ts
QuestState.ts
  ui/
PencilUI.ts
CrayonUI.ts
InteractionBubble.ts
SketchbookUI.ts
IconBubble.ts
  assets/
AssetLoader.ts
  data/
quests.ts
interactables.ts
colors.ts
```

# 씬 구조

## IntroScene

역할:

- 처음 스케치북 도입 연출 담당
- 팔이 올라와 오두막, 마을, 숲을 그리는 장면
- 스케치북 줌아웃
- CabinScene으로 전환

## CabinScene

역할:

- 오두막 내부
- 책상, 스케치북, 침대, 러그, 모닥불, 문 표시
- 스케치북 상호작용
- 외부 필드로 이동

## WorldScene

역할:

- 외부 마을/숲 필드
- 플레이어 이동
- 카메라 추적
- NPC와 아이템 상호작용
- 확장 단계에 따른 구역 활성화
- 최종 문 상호작용

## SketchbookScene

역할:

- 전체 화면 스케치북 UI
- 현재 진행도 표시
- 연필 적용
- 크레파스 적용
- 문 확인 및 문 완성 연출

## EndingScene

역할:

- 문 열림 이후 엔딩
- 아이가 침대에서 눈뜸
- 책상 위 완성된 스케치북 표시

# 전역 게임 상태

권장 상태 모델:

```ts
interface GameState {
  currentScene: SceneName;
  expansionLevel: number;
  pencilPieces: number;
  hasCompletedPencil: boolean;
  collectedCrayons: CrayonColor[];
  appliedColors: CrayonColor[];
  questStates: Record<string, QuestStatus>;
  hasSeenDoorInSketchbook: boolean;
  isFinalDoorUnlocked: boolean;
  isEndingTriggered: boolean;
}
```

## expansionLevel

```text
0: 초기 영역만 활성화
1: 숲 영역 1 활성화
2: 숲 영역 2 활성화
3: 숲 영역 3 활성화, 문 완성 가능
```

## pencilPieces

현재 연필 조각 개수다. 3개가 되면 `hasCompletedPencil`을 `true`로 설정한다.

## collectedCrayons

획득했지만 아직 적용하지 않은 크레파스를 포함한다.

## appliedColors

스케치북에서 적용되어 필드 색상 레이어가 활성화된 크레파스다.

# 입력 시스템

기본 입력:

- 이동: WASD 또는 방향키
- 상호작용: E

InputManager는 다음 상태를 제공한다.

```ts
interface InputState {
  moveX: number;
  moveY: number;
  interactPressed: boolean;
}
```

# 플레이어 이동

Player는 위치, 속도, 방향, 상호작용 범위를 가진다.

```ts
interface PlayerState {
  x: number;
  y: number;
  direction: Direction;
  speed: number;
  interactionRadius: number;
}
```

이동 중 충돌맵을 확인해 벽, 물, 아직 그려지지 않은 영역을 통과하지 못하게 한다.

# 카메라

WorldScene에서 카메라는 플레이어를 따라간다.

규칙:

- 플레이어 중심으로 이동
- 맵 경계에서 클램프
- 오두막 복귀 유도 등 특수 상황에서 짧은 시선 이동 가능

# 상호작용 시스템

모든 상호작용 대상은 공통 인터페이스를 가진다.

```ts
interface Interactable {
  id: string;
  x: number;
  y: number;
  radius: number;
  isActive(state: GameState): boolean;
  onInteract(game: Game): void;
  render(ctx: CanvasRenderingContext2D): void;
}
```

상호작용 거리 안에 있는 대상이 있으면 `InteractionBubble`을 플레이어 머리 위에 표시한다.

# NPC 시스템

NPC는 Interactable을 확장한다.

```ts
interface NPC extends Interactable {
  questId: string;
  iconSet: IconBubbleData;
}
```

NPC 상호작용 시 현재 퀘스트 상태에 맞는 아이콘 말풍선을 표시한다.

# 퀘스트 시스템

퀘스트 상태:

```ts
type QuestStatus =
  | 'locked'
  | 'available'
  | 'itemCollected'
  | 'completed'
  | 'rewardDropped'
  | 'rewardCollected';
```

퀘스트 데이터 예시:

```ts
interface QuestDefinition {
  id: string;
  npcId: string;
  requiredItemId: string;
  rewardType: 'pencilPiece' | 'crayon';
  rewardColor?: CrayonColor;
  unlockExpansionLevel: number;
}
```

# 연필 시스템

연필 조각 획득 시:

```text
pencilPieces += 1
if pencilPieces >= 3:
  hasCompletedPencil = true
  showCabinExclamation = true
```

스케치북에서 연필 적용 시:

```text
if hasCompletedPencil:
  expansionLevel += 1
  pencilPieces = 0
  hasCompletedPencil = false
  playDrawExpansionAnimation()
```

3차 확장 후에는 최종 문 상태를 활성화한다.

# 크레파스 시스템

크레파스 획득 시:

```text
collectedCrayons.push(color)
```

스케치북에서 크레파스 적용 시:

```text
color = collectedCrayons.shift()
appliedColors.push(color)
playColorApplyAnimation(color)
```

연필과 크레파스를 둘 다 가진 경우에는 연필 적용이 우선된다.

# 스케치북 상호작용 우선순위

```text
1. 완성된 연필이 있으면 새 숲 영역 그리기
2. 아니면 미적용 크레파스가 있으면 색 적용
3. 아니면 현재 스케치북 지도만 표시
```

# 렌더링 구조

Canvas 렌더링 순서:

```text
1. 배경 종이 질감
2. 맵 바닥과 길
3. 색상 레이어
4. 고정 오브젝트
5. NPC와 수집 오브젝트
6. 플레이어
7. 반짝임 효과
8. UI
```

# 색상 레이어

`appliedColors`에 따라 렌더링할 색상 레이어를 결정한다.

```ts
type CrayonColor = 'green' | 'brown' | 'skyBlue' | 'yellow';
```

MVP 필수 색상:

- green
- brown
- skyBlue

yellow는 선택 구현으로 둔다.

# 충돌 처리

MVP에서는 단순 사각형 충돌로 충분하다.

```ts
interface RectCollider {
  x: number;
  y: number;
  width: number;
  height: number;
}
```

충돌 대상:

- 오두막 벽
- 가구
- 나무
- 물
- 아직 해금되지 않은 숲 영역
- 맵 경계

# 확장 상태 처리

각 오브젝트 또는 구역은 필요한 expansionLevel을 가진다.

```ts
interface WorldObject {
  id: string;
  requiredExpansionLevel: number;
  render(...): void;
}
```

현재 `expansionLevel`보다 높은 오브젝트는 렌더링하지 않거나 흐릿한 밑그림으로만 렌더링한다.

# 에셋 로딩

AssetLoader는 SVG/PNG 에셋을 미리 로드한다.

필수 에셋:

- 키캡 아이콘
- 연필 조각
- 연필 UI 단계
- 크레파스 아이콘
- 퀘스트 아이콘 4종
- NPC 4종
- 오두막 문
- 최종 문

Canvas 도형으로 직접 그릴 수 있는 요소는 에셋 없이 구현 가능하다.

# 구현 제외

MVP에서 구현하지 않는다.

- 서버
- 로그인
- 저장 기능
- 전투
- 체력
- 제한 시간
- 상점
- 농사
- 복잡한 인벤토리
- 랜덤 생성
- 온라인 랭킹

# 개발 단계 추천

```text
1. Vite + TypeScript 프로젝트 생성
2. Canvas 게임 루프 구현
3. SceneManager 구현
4. Player 이동과 Camera 구현
5. CabinScene과 WorldScene 전환
6. Interactable 시스템 구현
7. PencilUI와 연필 조각 획득 구현
8. SketchbookScene과 expansionLevel 구현
9. NPC 퀘스트 구현
10. Crayon 시스템 구현
11. 최종 문과 EndingScene 구현
12. 도입 연출 추가
13. 아트와 애니메이션 개선
```

# 테스트 체크리스트

- 플레이어가 움직인다.
- 오두막 내부와 외부를 오갈 수 있다.
- 상호작용 키캡이 표시된다.
- NPC 퀘스트를 완료할 수 있다.
- 보상은 필드에 떨어지고 다시 상호작용해야 획득된다.
- 연필 조각 3개로 연필이 완성된다.
- 스케치북에서 숲이 확장된다.
- 크레파스 색상이 적용된다.
- 3차 확장 후 문이 등장한다.
- 문 상호작용 후 엔딩이 나온다.
