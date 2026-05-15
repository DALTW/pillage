# Pencil Village

`Pencil Village`는 아이가 그린 스케치북 세계를 탐험하고, 연필 조각과 크레파스로 세계를 완성해 현실로 돌아가는 브라우저 힐링 탐험 게임이다.

현재 저장소는 구현 전 기획 문서 단계다. 목표 구현 스택은 **Vite + TypeScript + Canvas API**이며, MVP는 약 10분 안에 도입부터 엔딩까지 완주 가능한 싱글플레이 브라우저 게임으로 정의되어 있다.

## 핵심 개념

- 플레이어는 통나무 오두막에서 시작해 자신이 그린 스케치북 세계로 들어간다.
- NPC 퀘스트와 필드 수집으로 연필 조각을 모은다.
- 연필 조각 3개로 연필 1개가 완성된다.
- 완성된 연필은 오두막의 스케치북에서 사용해 숲 영역을 확장한다.
- 크레파스는 스케치북에서 적용해 흑백 선화 세계에 색을 더한다.
- 숲을 3회 확장하면 최종 문이 등장하고, 문을 열면 엔딩으로 이어진다.

## MVP 범위

필수 구현 범위는 다음과 같다.

- 도입 스케치북 연출
- 오두막 내부와 외부 필드
- 플레이어 이동, 충돌, 카메라 추적
- 상호작용 키캡과 반짝임 힌트
- 아이콘 기반 NPC 퀘스트
- 연필 조각과 연필 UI
- 스케치북 확장 3단계
- 크레파스 획득과 색상 적용
- 최종 문과 엔딩

MVP에서는 저장, 서버, 로그인, 전투, 체력, 제한 시간, 상점, 농사, 복잡한 인벤토리, 랜덤 맵 생성은 제외한다.

## 문서 구조

프로젝트 기획과 구현 기준은 `docs/`에 정리되어 있다.

| 문서 | 용도 |
| --- | --- |
| `docs/README_Project_Document_Index.md` | 전체 문서 인덱스와 추천 읽기 순서 |
| `docs/Pencil_Village_Game_Concept.md` | 게임 정체성, 세계관, 핵심 재미 |
| `docs/Pencil_Village_Core_Gameplay.md` | 플레이 루프, 연필, 크레파스, 스케치북 규칙 |
| `docs/Pencil_Village_Interaction_UX.md` | 무문자 상호작용 UX와 아이콘 문법 |
| `docs/Pencil_Village_Narrative_Sequence.md` | 도입, 확장, 색칠, 엔딩 연출 |
| `docs/Pencil_Village_Map_And_Progression.md` | 오두막, 외부 필드, 숲 영역, 최종 문 위치 |
| `docs/Pencil_Village_NPC_And_Quest.md` | NPC 4종, 퀘스트, 보상 흐름 |
| `docs/Pencil_Village_Art_Direction.md` | 스케치북 질감, 연필 선화, 크레파스 색상 |
| `docs/Pencil_Village_Technical_Spec.md` | Vite + TypeScript + Canvas 구현 명세 |
| `docs/Pencil_Village_MVP_Scope.md` | 10분 MVP 필수 범위와 제외 범위 |
| `docs/Pencil_Village_Presentation_Points.md` | 해커톤 발표와 시연 포인트 |
| `docs/TODO.md` | 전체 프로젝트 목표와 완료 상태 관리 |

## 개발 우선순위

1. Vite + TypeScript 프로젝트 생성
2. Canvas 게임 루프와 입력 처리
3. 오두막 내부, 외부 필드, 씬 전환
4. 플레이어 이동, 충돌, 카메라
5. 상호작용 시스템
6. 연필 조각과 스케치북 확장
7. NPC 퀘스트
8. 크레파스 적용
9. 최종 문과 엔딩
10. 도입 연출과 아트 개선

## 현재 상태

- 기획 문서 작성됨
- 구현 프로젝트 scaffold는 아직 없음
- 진행 관리는 `docs/TODO.md`에서 체크박스로 갱신

## 작업 규칙

AI 에이전트와 개발자는 `AGENTS.md`를 먼저 확인한다. 작업 중 목표, 완료 기준, 범위 변경은 `docs/TODO.md`에 반영한다.
