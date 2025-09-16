# Claude Code 세션 컨텍스트

## 프로젝트 개요
- **프로젝트명**: TaxiManager
- **타입**: Windows Forms 애플리케이션
- **언어**: C#
- **GitHub 저장소**: https://github.com/ogosengi/taxi

## 요구사항
- **선행 작업**: GitHub과 동기화
- C# Windows Forms 애플리케이션
- 소스 주석은 항상 한글로 작성
- 코드는 최소한의 코드로 작성
- 소스 업데이트시 CLAUDE.md 파일도 항상 업데이트
- 소스 업데이트 시 GitHub에 업로드

### 기능 요구사항
- **택시 운행 관리 프로그램**
- **근무시간 관리**:
  - 근무 시작시간과 끝시간을 자유롭게 설정 가능
  - 다음날까지 이어지는 근무 지원 (예: 10시-다음날 2시)
  - 완전히 유연한 근무시간 패턴 지원
- **매출 관리**:
  - 일별 마감 방식의 매출 입력
  - 마감 후 매출 확인 및 관리 기능
- **운행 현황 및 정보 확인**:
  - 택시 운행 현황 조회 (월별/기간별)
  - 다양한 운행 통계 정보 확인

## 현재 상태 (2025-09-16)
### ✅ 완성된 기능들
- 초기 프로젝트 생성 및 GitHub 연동
- 완전히 유연한 근무시간 설정 기능
- 일별 마감 시스템 및 매출 관리
- 월별/기간별 통계 및 운행 현황 조회
- 일별 마감자료 조회/관리 기능
- 반응형 UI 레이아웃 (자동 크기 조정)
- SQLite 데이터베이스 도입 및 최적화
- 데이터 영속성 문제 해결

### 📊 품질 지표
- **빌드 상태**: 경고 0개, 오류 0개
- **코드 품질**: 47개 한글 XML 주석 적용
- **데이터베이스**: 최적화된 2개 테이블 구조
- **UI/UX**: 반응형 레이아웃 적용

## 핵심 아키텍처

### 모델 구조
```csharp
TaxiWorkShift    // 근무시간 정보 (유연한 시간 설정)
DailySettlement  // 일별 마감 정보 (매출 및 통계)
TaxiOperationStats // 운행 현황 통계
```

### 데이터베이스 스키마
```sql
-- 근무시간 테이블
CREATE TABLE WorkShifts (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Date TEXT NOT NULL,
    StartTime TEXT NOT NULL,
    EndTime TEXT NOT NULL,
    IsNightShift INTEGER NOT NULL,
    Revenue REAL NOT NULL,
    Notes TEXT
);

-- 일별마감 테이블
CREATE TABLE DailySettlements (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Date TEXT NOT NULL UNIQUE,
    SettlementDateTime TEXT NOT NULL,
    TotalRevenue REAL NOT NULL,
    TotalWorkingHours REAL NOT NULL,
    Notes TEXT
);
```

## 기술 스택
- **언어**: C#
- **프레임워크**: .NET 8.0, Windows Forms
- **데이터베이스**: SQLite (Microsoft.Data.Sqlite 9.0.9)
- **저장소**: GitHub (https://github.com/ogosengi/taxi)

## 주요 특징
1. **완전한 시간 유연성**: 다음날까지 이어지는 근무 지원
2. **일별 마감 시스템**: 체계적인 매출 관리
3. **반응형 UI**: 화면 크기 변경 시 자동 조정
4. **데이터 영속성**: 안정적인 SQLite 저장
5. **통합 관리**: 근무시간부터 매출 분석까지

## 개발 명령어
```bash
# 프로젝트 빌드
dotnet build

# 애플리케이션 실행
dotnet run --project TaxiManager

# 데이터베이스 위치
# D:\DEV\vibe\taxi\TaxiManager\bin\Debug\net8.0-windows\taxidata.db
```

## 작업 환경
- **작업 디렉토리**: D:\DEV\vibe\taxi
- **Git 브랜치**: master (origin 연결)
- **사용자**: ogosengi

---
*TaxiManager - 완전한 택시 운행 관리 솔루션*