---
title: "Pencil Village - Project Document Index"
subtitle: "전체 프로젝트 문서 인덱스"
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

# 문서 세트 개요

이 문서 세트는 브라우저 싱글플레이 힐링 탐험 게임 **Pencil Village**의 기획, UX, 연출, 맵, NPC, 아트, 기술 명세, MVP 범위, 발표 포인트를 분리해 정리한 프로젝트 문서다.

프로젝트는 **Vite + TypeScript + Canvas API** 기반으로 구현하는 것을 전제로 한다. 아트 리소스는 Canvas 선화와 AI 생성 SVG/PNG 에셋을 혼합한다.

# 문서 목록

## 1. Game Concept

파일: `Pencil_Village_Game_Concept.md`

용도:

- 게임의 정체성, 세계관, 핵심 재미를 정리한다.
- 팀원 또는 AI 코딩 도구가 프로젝트 목표를 빠르게 이해하도록 한다.

## 2. Core Gameplay

파일: `Pencil_Village_Core_Gameplay.md`

용도:

- 플레이 루프, 연필 조각 시스템, 크레파스 시스템, 스케치북 상호작용 규칙을 정의한다.
- 실제 게임 진행의 기본 규칙을 고정한다.

## 3. Interaction UX

파일: `Pencil_Village_Interaction_UX.md`

용도:

- 텍스트 없는 진행 방식, 상호작용 힌트, 아이콘 말풍선 문법을 정의한다.
- 플레이어가 어디로 가야 하는지 자연스럽게 이해하게 만드는 규칙을 정한다.

## 4. Narrative Sequence

파일: `Pencil_Village_Narrative_Sequence.md`

용도:

- 도입, 외부 전환, 첫 스케치북 확인, 확장, 색칠, 엔딩 연출을 장면 단위로 정리한다.
- 텍스트 없이 세계관을 전달하기 위한 연출 기준을 제공한다.

## 5. Map and Progression

파일: `Pencil_Village_Map_And_Progression.md`

용도:

- 오두막 내부, 외부 필드, 숲 영역 1-3, 최종 문의 위치와 진행 단계를 정의한다.
- 맵 확장과 카메라 구조를 구현 가능한 방식으로 정리한다.

## 6. NPC and Quest

파일: `Pencil_Village_NPC_And_Quest.md`

용도:

- 우체통 주민, 작은 새, 다람쥐, 큰 나무의 역할을 정의한다.
- 아이콘 기반 필수 퀘스트와 보상 방식을 정리한다.

## 7. Art Direction

파일: `Pencil_Village_Art_Direction.md`

용도:

- 스케치북 질감, 연필 선화, 색상 레이어, 에셋 제작 기준을 정의한다.
- AI 생성 에셋의 방향과 Canvas 표현 방식을 정리한다.

## 8. Technical Spec

파일: `Pencil_Village_Technical_Spec.md`

용도:

- Vite + TypeScript + Canvas 기반 구현 구조를 정의한다.
- 씬, 상태, 엔티티, 상호작용, 퀘스트, UI 시스템을 개발 명세로 정리한다.

## 9. MVP Scope

파일: `Pencil_Village_MVP_Scope.md`

용도:

- 10분 MVP에서 반드시 구현할 기능과 제외할 기능을 구분한다.
- 해커톤 시간 내 완성 가능한 우선순위를 제시한다.

## 10. Presentation Points

파일: `Pencil_Village_Presentation_Points.md`

용도:

- 해커톤 발표, 시연, 질의응답에서 강조할 내용을 정리한다.
- 프로젝트의 차별점과 AI 코딩 활용 포인트를 설명한다.

# 추천 사용 순서

기획을 검토할 때는 다음 순서로 읽는 것이 좋다.

```text
1. Game Concept
2. Core Gameplay
3. Narrative Sequence
4. Interaction UX
5. Map and Progression
6. NPC and Quest
7. Art Direction
8. MVP Scope
9. Technical Spec
10. Presentation Points
```

개발에 바로 들어갈 때는 다음 문서를 우선 읽는다.

```text
1. Technical Spec
2. Core Gameplay
3. Map and Progression
4. Interaction UX
5. MVP Scope
```

발표 준비를 할 때는 다음 문서를 우선 읽는다.

```text
1. Game Concept
2. Narrative Sequence
3. Presentation Points
```

# 현재 확정된 핵심 결정

- 제목: **Pencil Village**
- 장르: 힐링, 탐험, 발전
- 플랫폼: 브라우저 싱글플레이
- 구현 스택: Vite + TypeScript + Canvas API
- 화면 방식: 탑다운 2D, 약간 사선에서 보는 시점
- 아트: 스케치북 질감, 연필 선화, 크레파스 색상 레이어
- 진행: 연필 조각 3개로 연필 완성, 연필 1개로 숲 영역 1개 확장
- 확장: 숲 영역 1, 숲 영역 2, 숲 영역 3
- 목표: 숲 끝에 등장하는 오두막 문과 닮은 문을 열고 현실로 돌아가기
- 텍스트: 일반 대사와 튜토리얼 문장 최소화, 아이콘 중심 진행
- 저장 기능: MVP에서는 없음
