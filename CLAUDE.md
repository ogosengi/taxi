# Claude Code 세션 컨텍스트

## 프로젝트 개요
- **프로젝트명**: TaxiManager
- **타입**: Windows Forms 애플리케이션
- **언어**: C#
- **세션 컨텍스트**: CLAUDE.md(현재 파일). 소스 업데이트시 이 파일도 항상 업데이트
- **GitHub 저장소**: https://github.com/ogosengi/taxi


## 요구사항
- C# Windows Forms 애플리케이션
- 소스 주석은 항상 한글로 작성
- 코드는 최소한의 코드로 작성

### 기능 요구사항
- **택시 운행 관리 프로그램**
- **근무시간 관리**:
  - 근무 시작시간과 끝시간을 자유롭게 설정 가능
  - 다음날까지 이어지는 근무 지원 (예: 10시-다음날 2시)
  - 중간에 쉬는 시간 설정 가능
  - 완전히 유연한 근무시간 패턴 지원
- **매출 관리**:
  - 근무시간별 매출 입력
  - 마감 후 매출 확인 기능
- **운행 현황 및 정보 확인**:
  - 택시 운행 현황 조회
  - 다양한 운행 정보 확인

## 현재 상태 (2025-09-15)
- ✅ 초기 프로젝트 생성 완료
- ✅ GitHub 저장소 생성 및 코드 업로드 완료
- ✅ Git 초기 설정 완료 (master 브랜치)
- ✅ 요구사항에 맞는 택시 관리 애플리케이션 구현 완료
- ✅ 완전히 유연한 근무시간 설정 기능 구현
- ✅ 운행 현황 통계 및 매출 분석 기능 구현
- ✅ UI 단순화 및 SQLite 데이터베이스 도입
- ✅ 빌드 및 실행 테스트 성공 (오류 0개)

## 작업 기록
### 2025-09-15 (초기 설정)
1. GitHub CLI 설치 확인
2. GitHub 계정 인증 (ogosengi)
3. 퍼블릭 저장소 생성: taxi
4. 로컬 코드를 GitHub에 푸시
5. CLAUDE.md 컨텍스트 파일 생성

### 2025-09-15 (기능 구현)
1. TaxiWorkShift 모델 업데이트 - 완전 유연한 근무시간 지원
2. BreakMinutes 속성 추가 - 중간 휴식시간 관리
3. TaxiOperationStats 모델 생성 - 운행 현황 통계
4. TaxiDataService 확장 - 기간별 매출/통계 조회 기능
5. Form1 UI 업데이트 - 휴식시간 입력, 기간별 통계 조회
6. 빌드 테스트 및 오류 수정 완료

### 2025-09-15 (UI 단순화 및 SQLite 도입)
1. 근무타입 드롭다운 제거 - 사용자 인터페이스 단순화
2. 시간 입력 형식을 HH:00으로 고정 - 분은 항상 00으로 설정
3. 휴식시간 관리 기능 제거 - 근무시간만 관리하도록 단순화
4. JSON 파일 저장소를 SQLite 데이터베이스로 교체
5. Microsoft.Data.Sqlite NuGet 패키지 추가
6. TaxiDataService 완전히 재작성 - SQLite CRUD 작업 구현
7. TaxiWorkShift 모델 단순화 - BreakMinutes, ActualWorkingHours 제거
8. 데이터베이스 자동 초기화 및 테이블 생성 기능 구현
9. 데이터베이스 테이블 및 컬럼명을 한글로 변경
10. 최종 빌드 및 실행 테스트 성공

## 다음 세션을 위한 정보
- 작업 디렉토리: D:\DEV\vibe\taxi
- Git 상태: master 브랜치, origin과 연결됨
- 사용자: ogosengi

## 기술 스택
- **언어**: C#
- **프레임워크**: .NET 8.0, Windows Forms
- **데이터베이스**: SQLite (Microsoft.Data.Sqlite)
- **저장소**: GitHub (https://github.com/ogosengi/taxi)

## 추천 명령어
```bash
# 프로젝트 빌드
dotnet build

# 애플리케이션 실행
dotnet run --project TaxiManager

# 데이터베이스 파일 위치
# D:\DEV\vibe\taxi\TaxiManager\bin\Debug\net8.0-windows\taxidata.db
```

## 데이터베이스 스키마
```sql
CREATE TABLE 근무시간 (
    아이디 INTEGER PRIMARY KEY AUTOINCREMENT,
    날짜 TEXT NOT NULL,
    시작시간 TEXT NOT NULL,
    종료시간 TEXT NOT NULL,
    야간근무여부 INTEGER NOT NULL,
    매출 REAL NOT NULL,
    메모 TEXT,
    마감여부 INTEGER NOT NULL
);
```

---
*이 파일은 Claude Code 세션 간 컨텍스트 유지를 위해 자동 생성되었습니다.*