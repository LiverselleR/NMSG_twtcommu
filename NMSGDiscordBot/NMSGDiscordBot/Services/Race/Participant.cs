using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Numerics;

namespace NMSGDiscordBot
{
    public partial class Participant : IComparable
    {
        public String name;                             // 우마무스메 이름
        public RunningStyle runningStyle;               // 우마무스메 각질

        public Derby derby;                             // 더비 정보
        public Racetrack racetrack;                     // 더비 경기장 정보

        public int rank;                                // 더비 중 현재 순위
        public int prevRank;                            // 더비 중 전 턴 순위

        public Vector2 currPosition;                    // 더비용 현재 턴 포지션
        public Vector2 prevPosition;                    // 더비용 이전 턴 포지션
        public CoursePhase coursePhase;                 // 더비용 현재 페이즈

        public Boolean isGoal;                          // 더비용 현재 골 여부 확인
        public double goalTiming;                       // 더비용 골 타이밍 (골인한 경우 갱신)

        private double calibratedSpeed;                 // 보정 스피드
        private double calibratedStamina;               // 보정 체력
        private double calibratedPower;                 // 보정 파워
        private double calibratedToughness;             // 보정 근성
        private double calibratedIntelligence;          // 보정 지능

        private double defaultTargetSpeed;              // 기본 목표 속도
        private double normalTargetSpeed;               // 일반 목표 속도
        private double spurtTargetSpeed;                // 스퍼트 목표 속도
        private double minimumTargetSpeed;              // 최소 목표 속도
        private double maximumTargetSpeed;              // 최대 목표 속도
        private double targetSpeed;                     // 목표 속도 (위의 목표 속도들을 이용한 목표 속도)

        private double lengthAptitudeSpeedValue;        // 거리 적성 스피드값
        private double lengthAptitudePowerValue;        // 거리 적성 파워값
        private double fieldTypeAccelCalibrateValue;    // 마정 적성 가속 보정값


        private Boolean isPositionKeep;                 // 포지션 킵 여부
        private Boolean isForceInMove;                  // 포지션 무브 여부
        private PositionKeep positionKeep;              // 포지션 킵 상태 (현재 포지션 유지하면서 달림)
        private ForceInMove forceInMove;                // 포지션 무브 상태 (다른 코스로 전환하면서 달림)

        private Boolean isBlocked;                      // 블록당함 여부
        private Boolean isSurrounded;                   // 포위당함 여부
        private Boolean isFrontBlocked;                 // 정면 블록 여부
        private Boolean isInsideBlocked;                // 인-라인 블록 여부
        private Boolean isOutsideBlocked;               // 아웃-라인 블록 여부
        private Boolean isFever;                        // 흥분 여부
        private Boolean isSpurt;                        // 스퍼트 여부
        private Boolean isStartAccel;                   // 출발 스퍼트 가속 여부

        private List<Participant> surroundParticipants; // 주변 둘러싼 우마무스메들 정보 (마군에 끼어 있을 경우에만)

        private double maxStamina;                      // 더비용 최대 스테미나
        private double currStamina;                     // 더비용 현재 스테미나
        public double currSpeed;                        // 더비용 현재 속도
        private double currAccel;                       // 더비용 현재 가속도 

        public Participant(Umamusume u,
                           Derby d,
                           RunningStyle runningStyle,
                           float currPosition)
        {
            name = u.name;
            this.runningStyle = runningStyle;

            derby = d;
            racetrack = d.racetrack;

            rank = 0;
            prevRank = 0;

            this.currPosition = new Vector2(0, currPosition);
            this.prevPosition = this.currPosition;
            coursePhase = CoursePhase.First;

            isGoal = false;
            goalTiming = -1;

            // 보정 스탯
            calibratedSpeed = u.speed * GetCalibratingSpeedValue(d.statusType, u) + GetFieldTypeAptitudeSpeedValue();
            calibratedIntelligence = u.intelligence * GetRunningStyleAptitudeValue(u);
            calibratedPower = u.power + GetFieldTypeAptitudePowerValue();
            calibratedStamina = u.stamina;
            calibratedToughness = u.toughness;

            lengthAptitudePowerValue = GetLengthAptitudePowerValue(u);
            lengthAptitudeSpeedValue = GetLengthAptitudeSpeedValue(u);

            // 목표 속도
            /// 기본 목표 속도
            defaultTargetSpeed = GetDefaultTargetSpeed();
            /// 일반 목표 속도
            normalTargetSpeed = GetNormalTargetSpeed();
            /// 스퍼트 목표 속도
            spurtTargetSpeed = GetSpurtTargetSpeed();
            /// 최대 목표 속도
            maximumTargetSpeed = 30;
            /// 최소 목표 속도
            minimumTargetSpeed = GetMinimumTargetSpeed();
            /// 실제 목표 속도
            targetSpeed = normalTargetSpeed;

            // 필드 적성 가속도 보정치
            fieldTypeAccelCalibrateValue = GetFieldTypeAccelCalibrateValue(u);

            // 포지션 킵/무브 여부 초기화
            isPositionKeep = false;
            isForceInMove = false;
            positionKeep = PositionKeep.non;
            forceInMove = ForceInMove.non;

            isBlocked = false;
            isSurrounded = false;
            isFrontBlocked = false;
            isInsideBlocked = false;
            isOutsideBlocked = false;
            isFever = false;
            isSpurt = false;
            isStartAccel = true;

            surroundParticipants = new List<Participant>();
            
            // 최대 스태미너 초기화
            maxStamina = GetMaximumStamina();
            currStamina = u.stamina;
            currSpeed = 0;
            currAccel = 0;

        }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            Participant otherParticipant = obj as Participant;
            if (otherParticipant != null)
            {
                if (this.isGoal && otherParticipant.isGoal) return this.rank.CompareTo(otherParticipant.rank);
                else if (this.isGoal) return -1;
                else if (otherParticipant.isGoal) return 1;
                else return this.currPosition.X.CompareTo(otherParticipant.currPosition.X) * -1;
            }
            else
                throw new ArgumentException("Object is not Participant");
        }

        public void TurnProcess(List<Participant> pList)
        {
            if (isGoal) return;
            TurnActionDecide(pList);
            TurnActionActivate();
            if (TurnSpecialSituationCheck(pList))
                TurnSpecialSituationProcess(pList);
            return;
        }
        public void TurnActionDecide(List<Participant> pList)
        {
            coursePhase = derby.GetCoursePhase(currPosition.X);

            CheckNear(pList);
            switch(runningStyle)
            {
                case RunningStyle.Runaway:
                    ActionDecideRunaway(pList);
                    break;
                case RunningStyle.Front:
                    ActionDecideFront(pList);
                    break;
                case RunningStyle.FI:
                    ActionDecideFI(pList);
                    break;
                case RunningStyle.Stretch:
                    ActionDecideStretch(pList);
                    break;
                default:
                    break;
            }
            UpdateTargetSpeed();
            if (isStartAccel && targetSpeed <= currSpeed)
                isStartAccel = false;
            return;
        }

        public void TurnActionActivate()
        {
            currAccel = GetAccel();
            currSpeed += currAccel / 20;
            prevPosition = currPosition;

            if (isForceInMove)
            {
                switch (forceInMove)
                {
                    case ForceInMove.InsideCatchUp:
                    case ForceInMove.InsideOvertaking:
                    case ForceInMove.InsideMove:
                        {
                            currPosition.X += (float)currSpeed / 20;
                            currPosition.Y -= 0.1f;
                        }
                        break;
                    case ForceInMove.OutsideOvertaking:
                    case ForceInMove.OutsideMove:
                    case ForceInMove.OutsideCatchUp:
                        {
                            currPosition.X += (float)currSpeed / 20;
                            currPosition.Y += 0.1f;
                        }
                        break;
                    default:
                        currPosition.X += (float)currSpeed;
                        break;

                }
            }
            else
                currPosition.X += (float)currSpeed / 20;

            currStamina -= GetStaminaExhaustionSpeed(derby.turfCondition);

            return;
        }

        public Boolean TurnSpecialSituationCheck(List<Participant> pList)
        {
            return false;
        }
        public void TurnSpecialSituationProcess(List<Participant> pList)
        {
            return;
        }

        public void GoalCheck()
        {
            if (isGoal) return;
            else if(currPosition.X >= derby.courseLength)
            {
                isGoal = true;
                goalTiming = (derby.courseLength - prevPosition.X) / (currPosition.X - prevPosition.X);
            }
        }

        public void RankRenewal(int currRank)
        {
             prevRank = this.rank;
             this.rank = currRank;
             GoalCheck();    
        }

        // 기본 속도 보정 - 더비에서 정해진 스탯 값에 따라 기본 보정
        private double GetCalibratingSpeedValue(StatusType statusType, Umamusume u)
        {
            double result;
            int standard;

            switch (statusType)
            {
                case StatusType.intelligence:
                    standard = u.intelligence;
                    break;
                case StatusType.power:
                    standard = u.power;
                    break;
                case StatusType.speed:
                    standard = u.speed;
                    break;
                case StatusType.stamina:
                    standard = u.stamina;
                    break;
                case StatusType.toughness:
                    standard = u.toughness;
                    break;
                default:
                    standard = 0;
                    break;
            }

            switch (standard / 300)
            {
                case 0:
                    result = 1.00;
                    break;
                case 1:
                    result = 1.05;
                    break;
                case 2:
                    result = 1.10;
                    break;
                case 3:
                    if (standard < 1000) result = 1.15;
                    else result = 1.20;
                    break;
                default:
                    result = 1.00;
                    break;

            }

            return result;
        }

        // 경마장 상태 보정 - 스피드
        private double GetFieldTypeAptitudeSpeedValue()
        {
            switch (derby.turfCondition)
            {
                case TurfCondition.bad:
                    return -50.0;
                default:
                    return 0;
            }
        }
        // 경마장 상태 보정 - 파워
        private double GetFieldTypeAptitudePowerValue()
        {
            switch (derby.turfCondition)
            {
                case TurfCondition.normal:
                    return 0;
                default:
                    return -50.0;
            }
        }
        // 각질 적성 보정
        private double GetRunningStyleAptitudeValue(Umamusume u)
        {
            Aptitude aptitude;

            switch (runningStyle)
            {
                case RunningStyle.Runaway:
                    aptitude = u.runawayAptitude;
                    break;
                case RunningStyle.Front:
                    aptitude = u.frontAptitude;
                    break;
                case RunningStyle.FI:
                    aptitude = u.fiAptitude;
                    break;
                case RunningStyle.Stretch:
                    aptitude = u.stretchAptitude;
                    break;
                default:
                    aptitude = u.runawayAptitude;
                    break;
            }

            switch (aptitude)
            {
                case Aptitude.S:
                    return 1.10;
                case Aptitude.A:
                    return 1.00;
                case Aptitude.B:
                    return 0.85;
                case Aptitude.C:
                    return 0.75;
                case Aptitude.D:
                    return 0.60;
                case Aptitude.E:
                    return 0.40;
                case Aptitude.F:
                    return 0.20;
                case Aptitude.G:
                    return 0.10;
                default:
                    return 0.10;

            }
        }



        // 기본 목표 속도 계산
        private double GetDefaultTargetSpeed()
        {
            return (Math.Sqrt(500 * calibratedSpeed)) * lengthAptitudeSpeedValue * 0.002 + GetRaceReferenceSpeed()
                * (GetRunningStyleCalibratingValue() + GetIntelligenceRandomCalibratingValue());
        }
        /// 레이스 타입 확인
        private Aptitude GetLengthAptitude(Umamusume u)
        {
            Aptitude aptitude;
            if (derby.courseLength < 1600)
            {
                aptitude = u.shortAptitude;
            }
            else if (derby.courseLength < 2000)
            {
                aptitude = u.mileAptitude;
            }
            else if (derby.courseLength <= 2400)
            {
                aptitude = u.middleAptitude;
            }
            else // length > 2400
            {
                aptitude = u.longAptitude;
            }

            return aptitude;
        }
        /// 거리 적성 보정 - 스피드  
        private double GetLengthAptitudeSpeedValue(Umamusume u)
        {
            Aptitude aptitude;

            aptitude = GetLengthAptitude(u);

            switch (aptitude)
            {
                case Aptitude.S:
                    return 1.05;
                case Aptitude.A:
                    return 1.00;
                case Aptitude.B:
                    return 0.90;
                case Aptitude.C:
                    return 0.80;
                case Aptitude.D:
                    return 0.60;
                case Aptitude.E:
                    return 0.40;
                case Aptitude.F:
                    return 0.20;
                case Aptitude.G:
                    return 0.10;
                default:
                    return 0.10;

            }
        }
        /// 거리 적성 보정 - 파워
        private double GetLengthAptitudePowerValue(Umamusume u)
        {
            Aptitude aptitude;

            aptitude = GetLengthAptitude(u);

            switch (aptitude)
            {
                case Aptitude.E:
                    return 0.60;
                case Aptitude.F:
                    return 0.50;
                case Aptitude.G:
                    return 0.40;
                default:
                    return 1.00;
            }
        }
        /// 레이스 기준속도 (레이스 길이에 따라 달라짐)
        private double GetRaceReferenceSpeed()
        {
            double result = 20;

            result += (2000 - derby.courseLength) / 100;

            return result;
        }
        /// 각질 속도 보정
        private double GetRunningStyleCalibratingValue()
        {
            switch (runningStyle)
            {
                case RunningStyle.Runaway:
                    {
                        if (isSpurt) 
                            return 0.962;
                        else if (coursePhase == CoursePhase.First)
                            return 1.000;
                        else if (coursePhase == CoursePhase.Middle)
                            return 0.980;
                        else  /* coursePhase == CoursePhase.Last */
                            return 0.962;
                    }
                case RunningStyle.Front:
                    {
                        if (isSpurt)
                            return 0.975;
                        else if (coursePhase == CoursePhase.First)
                            return 0.978;
                        else if (coursePhase == CoursePhase.First) 
                            return 0.991;
                        else /* coursePhase == CoursePhase.First */ 
                            return 0.975;
                    }
                case RunningStyle.FI:
                    {
                        if (isSpurt)
                            return 0.994;
                        else if (coursePhase == CoursePhase.First)
                            return 0.938;
                        else if (coursePhase == CoursePhase.Middle)
                            return 0.998;
                        else /*coursePhase == CoursePhase.Last */
                            return 0.994;
                    }
                case RunningStyle.Stretch:
                    {
                        if (!isSpurt && coursePhase == CoursePhase.First) return 0.931;
                        else return 1.000;
                    }
                default:
                    return 1.000;
            }
        }
        /// 지능 랜덤 보정
        private double GetIntelligenceRandomCalibratingValue()
        {
            Random random = new Random();
            double maximum = (calibratedIntelligence / 5500) * Math.Log(calibratedIntelligence * 0.1);
            double minimum = maximum - 0.65;
            return minimum + random.NextDouble() * 0.65;
        }

        // 일반 목표 속도 계산
        private double GetNormalTargetSpeed()
        {
            if (isPositionKeep)
            {
                switch (positionKeep)
                {
                    case PositionKeep.SpeedUp:
                        return defaultTargetSpeed * 1.04;
                    case PositionKeep.Overtaking:
                        return defaultTargetSpeed * 1.05;
                    case PositionKeep.FaceDown:
                        return defaultTargetSpeed * 0.915;
                    case PositionKeep.FaceUp:
                        return defaultTargetSpeed * 1.04;
                    default:
                        return defaultTargetSpeed;
                }
            }
            else if (isForceInMove)
            {
                double result;
                Random rand = new Random();
                switch (forceInMove)
                {
                    case ForceInMove.OutsideOvertaking:
                    case ForceInMove.InsideOvertaking:
                        result = 1.01;
                        break;
                    case ForceInMove.InsideMove:
                    case ForceInMove.OutsideMove:
                        result = 1.01;
                        break;
                    case ForceInMove.InsideCatchUp:
                    case ForceInMove.OutsideCatchUp:
                        result = 1.03;
                        break;
                    default:
                        result = 1;
                        break;
                }
                return defaultTargetSpeed * (result + rand.NextDouble() * 0.10);
            }
            else return defaultTargetSpeed;
        }

        // 스퍼트 목표 속도 계산
        private double GetMaximumSpurtTargetSpeed()
        {
            return 1.05 * defaultTargetSpeed + 0.01 * GetRaceReferenceSpeed() + Math.Sqrt(500 * calibratedSpeed) * lengthAptitudeSpeedValue * 0.002;
        }

        // 최소 목표 속도 계산
        private double GetMinimumTargetSpeed()
        {
            return GetRaceReferenceSpeed() * 0.85 + Math.Sqrt(200 * calibratedToughness) * 0.001;
        }

        // 블록 당하고 있을 때 최대 목표 속도 계산
        private double GetMaximumTargetSpeed(Participant p)
        {
            Random rand = new Random();
            return (0.988 + rand.NextDouble() * 0.012) * p.currSpeed;
        }
        // 현재 타겟 속도 계산
        private void UpdateTargetSpeed()
        {
            defaultTargetSpeed = GetDefaultTargetSpeed();
            normalTargetSpeed = GetNormalTargetSpeed();
            if(!isSpurt)
                spurtTargetSpeed = GetSpurtTargetSpeed();
            // maximumTargetSpeed : CheckNear 에서 확인
            if (currStamina <= 0)
                targetSpeed = minimumTargetSpeed;
            else if (isSpurt)
                targetSpeed = spurtTargetSpeed;
            else if (spurtTargetSpeed > normalTargetSpeed)
                targetSpeed = spurtTargetSpeed;
            else if (spurtTargetSpeed < normalTargetSpeed)
            {
                if (normalTargetSpeed > maximumTargetSpeed && !isForceInMove && !isPositionKeep)
                    targetSpeed = maximumTargetSpeed;
                else
                    targetSpeed = normalTargetSpeed;
            }
                
            
        }

        // 가속도 계산
        private double GetAccel()
        {
            double result;

            if (currSpeed > targetSpeed)
                result = GetDeceleration();
            else if (currSpeed < targetSpeed)
            {
                result = GetRunningStyleAccelCalibrateValue() * 0.0006 * Math.Sqrt(500 * calibratedPower) * fieldTypeAccelCalibrateValue;
            }
            else result = 0;

            if (isStartAccel) return result + 24;
            else return result;
        }

        // 각질 가속도 보정
        private double GetRunningStyleAccelCalibrateValue()
        {
            switch (runningStyle)
            {
                case RunningStyle.Runaway:
                    {
                        if (isSpurt)
                            return 0.996;
                        else if (coursePhase == CoursePhase.First) 
                            return 1.000;
                        else if (coursePhase == CoursePhase.Middle)
                            return 1.000;
                        else /* coursePhase == CoursePhase.Last */ 
                            return 0.996;
                    }
                case RunningStyle.Front:
                    {
                        if (isSpurt)
                            return 0.996;
                        else if (coursePhase == CoursePhase.First)
                            return 0.985;
                        else if (coursePhase == CoursePhase.Middle)
                            return 1.000;
                        else /* coursePhase == CoursePhase.Last */
                            return 0.996;
                    }
                case RunningStyle.FI:
                    {
                        if (isSpurt)
                            return 1.000;
                        else if (coursePhase == CoursePhase.First)
                            return 0.975;
                        else if (coursePhase == CoursePhase.Middle)
                            return 1.000;
                        else /* coursePhase == CoursePhase.Last */
                            return 1.000;
                    }
                case RunningStyle.Stretch:
                    {
                        if (isSpurt)
                            return 0.997;
                        else if (coursePhase == CoursePhase.First)
                            return 0.945;
                        else if (coursePhase == CoursePhase.Middle)
                            return 1.000;
                        else /* coursePhase == CoursePhase.Last */
                            return 0.997;
                    }
                default:
                    return 1.000;
            }
        }
        // 마장 적성 보정
        private double GetFieldTypeAccelCalibrateValue(Umamusume u)
        {
            Aptitude aptitude;
            if (racetrack.fieldType == FieldType.grass) aptitude = u.grassAptitude;
            else aptitude = u.durtAptitude;

            switch (aptitude)
            {
                case Aptitude.S:
                    return 1.05;
                case Aptitude.A:
                    return 1.00;
                case Aptitude.B:
                    return 0.90;
                case Aptitude.C:
                    return 0.80;
                case Aptitude.D:
                    return 0.70;
                case Aptitude.E:
                    return 0.50;
                case Aptitude.F:
                    return 0.30;
                case Aptitude.G:
                    return 0.10;
                default:
                    return 0.10;

            }
        }
        // 감속 계산
        private double GetDeceleration()
        {
            if (currStamina <= 0) return 1.2;
            else if (coursePhase == CoursePhase.First) return 1.2;
            else if (coursePhase == CoursePhase.Middle) return 0.8;
            else return 1.0;
        }

        // 최대 체력 계산
        private double GetMaximumStamina()
        {
            return derby.courseLength + 0.8 * GetRunningStyleStaminaCalibrateValue() * calibratedStamina;
        }
        // 각질 체력 보정
        private double GetRunningStyleStaminaCalibrateValue()
        {
            switch (runningStyle)
            {
                case RunningStyle.Runaway:
                    return 0.95;
                case RunningStyle.Front:
                    return 0.89;
                case RunningStyle.FI:
                    return 1.00;
                case RunningStyle.Stretch:
                    return 0.995;
                default:
                    return 1.00;
            }
        }


        // 턴당 체력 소모 속도 (20턴 == 1초)
        private double GetStaminaExhaustionSpeed(TurfCondition turfCondition)
        {
            return GetUmamusumeStatusCalibrateValue() * (Math.Pow((currSpeed - GetRaceReferenceSpeed() + 12), 2) / 144)
                * GetTurfConditionStaminaCalibrateValue(turfCondition) * GetSpurtStaminaCalibrateValue();
        }
        // 체력 소모 속도 말 상태 보정
        private double GetUmamusumeStatusCalibrateValue()
        {
            if (isFever) return 1.6;
            else if (isPositionKeep || positionKeep == PositionKeep.FaceDown) return 0.6;
            else return 1;
        }
        // 마장 상태 체력 보정
        private double GetTurfConditionStaminaCalibrateValue(TurfCondition turfCondition)
        {
            if(racetrack.fieldType == FieldType.grass)
            {
                switch (turfCondition)
                {
                    case TurfCondition.heavy:
                    case TurfCondition.bad:
                        return 1.02;
                    default:
                        return 1.00;
                }
            }
            else // fieldType == FieldType.durt
            {
                switch (turfCondition)
                {
                    case TurfCondition.heavy:
                        return 1.01;
                    case TurfCondition.bad:
                        return 1.02;
                    default:
                        return 1.00;
                }
            }
        }
        // 스퍼트 체력 보정
        private double GetSpurtStaminaCalibrateValue()
        {
            if (isSpurt) return (1 + 200 / Math.Sqrt(600 * calibratedToughness));
            else return 1;
        }

        private void CheckNear(List<Participant> pList)
        {
            int frontBlockCount = 0;
            int backwardBlockCount = 0;

            isBlocked = false;
            isSurrounded = false;
            isFrontBlocked = false;
            isInsideBlocked = false;
            isOutsideBlocked = false;
            maximumTargetSpeed = 30;

            if (currPosition.Y < 0.5) isInsideBlocked = true;

            for(int i = rank - 1; i > 0; i--)
            {
                if (pList[i].isGoal) continue;

                Vector2 v = pList[i].currPosition - currPosition;
                if(v.Length() < 1)
                {
                    if(!isBlocked)
                    {
                        isBlocked = true;
                        maximumTargetSpeed = GetMaximumTargetSpeed(pList[i]);
                    }
                    if (v.Y < 0.5)
                        isInsideBlocked = true;
                    else if (v.Y > -0.5)
                        isOutsideBlocked = true;
                    else
                        isFrontBlocked = true;

                    frontBlockCount++;
                    surroundParticipants.Add(pList[i]);
                }
            }

            for (int i = rank + 1; i < pList.Count; i++)
            {
                Vector2 v = pList[i].currPosition - currPosition;
                if (v.Length() < 1) backwardBlockCount++;
                surroundParticipants.Add(pList[i]);
            }

            if ((frontBlockCount >= 3 && backwardBlockCount >= 3)
                || (currPosition.Y <= 1 && frontBlockCount >= 2 && backwardBlockCount >= 2))
                isSurrounded = true;
            else surroundParticipants.Clear();

            return;
        }
        private void ActionDecideRunaway(List<Participant> pList)
        {
            switch (coursePhase)
            {
                case CoursePhase.First:
                    if (rank < pList.Count - 1)
                    {
                        if (rank > 0)
                        {
                            if (isBlocked)
                            {
                                if (!isInsideBlocked)
                                {
                                    isPositionKeep = false;
                                    isForceInMove = true;
                                    positionKeep = PositionKeep.non;
                                    forceInMove = ForceInMove.InsideOvertaking;
                                }
                                else if (!isFrontBlocked)
                                {
                                    isPositionKeep = true;
                                    isForceInMove = false;
                                    positionKeep = PositionKeep.Overtaking;
                                    forceInMove = ForceInMove.non;
                                }
                                else
                                {
                                    isPositionKeep = false;
                                    isForceInMove = true;
                                    positionKeep = PositionKeep.non;
                                    forceInMove = ForceInMove.OutsideOvertaking;
                                }
                            }
                            else
                            {
                                isPositionKeep = true;
                                isForceInMove = false;
                                positionKeep = PositionKeep.Overtaking;
                                forceInMove = ForceInMove.non;
                            }
                        }
                        else if (pList[rank + 1].runningStyle == RunningStyle.Runaway
                            || (pList[rank + 1].runningStyle != RunningStyle.Runaway && currPosition.X - pList[rank + 1].currPosition.X < 10))
                        {
                            if (!isInsideBlocked)
                            {
                                isPositionKeep = false;
                                isForceInMove = true;
                                positionKeep = PositionKeep.non;
                                forceInMove = ForceInMove.InsideMove;
                            }
                            else
                            {
                                isPositionKeep = true;
                                isForceInMove = false;
                                positionKeep = PositionKeep.SpeedUp;
                                forceInMove = ForceInMove.non;
                            }
                        }
                        else // pList[rank + 1].runningStyle != RunningStyle.Runaway && currPosition.X - pList[rank + 1].currPosition.X >= 10
                        {
                            if (!isInsideBlocked)
                            {
                                isPositionKeep = false;
                                isForceInMove = true;
                                positionKeep = PositionKeep.non;
                                forceInMove = ForceInMove.InsideMove;
                            }
                            else
                            {
                                isPositionKeep = true;
                                isForceInMove = false;
                                positionKeep = PositionKeep.SpeedUp;
                                forceInMove = ForceInMove.non;
                            }
                        }
                    }
                    break;
                default:
                    if (!isInsideBlocked)
                    {
                        isPositionKeep = false;
                        isForceInMove = true;
                        positionKeep = PositionKeep.non;
                        forceInMove = ForceInMove.InsideMove;
                    }
                    else
                    {
                        isPositionKeep = false;
                        isForceInMove = false;
                        positionKeep = PositionKeep.non;
                        forceInMove = ForceInMove.non;
                    }
                    break;
            }
        }
        private void ActionDecideFront(List<Participant> pList)
        {
            switch (coursePhase)
            {
                case CoursePhase.First:
                    if (rank > 0)
                    {
                        if (pList[rank - 1].runningStyle == RunningStyle.Runaway && pList[rank - 1].currPosition.X - currPosition.X < 3)
                        {
                            isPositionKeep = true;
                            isForceInMove = false;
                            positionKeep = PositionKeep.FaceDown;
                            forceInMove = ForceInMove.non;
                        }
                        else if (pList[rank - 1].runningStyle == RunningStyle.Runaway && pList[rank - 1].currPosition.X - currPosition.X > 6)
                        {
                            isPositionKeep = true;
                            isForceInMove = false;
                            positionKeep = PositionKeep.FaceUp;
                            forceInMove = ForceInMove.non;
                        }
                        else if (pList[rank - 1].runningStyle == RunningStyle.Runaway)
                        {
                            if (!isInsideBlocked)
                            {
                                isPositionKeep = false;
                                isForceInMove = true;
                                positionKeep = PositionKeep.non;
                                forceInMove = ForceInMove.InsideMove;
                            }
                            else
                            {
                                isPositionKeep = false;
                                isForceInMove = false;
                                positionKeep = PositionKeep.non;
                                forceInMove = ForceInMove.non;
                            }
                        }
                        else if (isSurrounded)
                        {
                            isPositionKeep = false;
                            isForceInMove = false;
                            positionKeep = PositionKeep.non;
                            forceInMove = ForceInMove.non;
                        }
                        else if (isBlocked)
                        {
                            isPositionKeep = false;
                            isForceInMove = true;
                            positionKeep = PositionKeep.non;
                            forceInMove = ForceInMove.OutsideOvertaking;
                        }
                        else
                        {
                            if (isInsideBlocked)
                            {
                                isPositionKeep = false;
                                isForceInMove = true;
                                positionKeep = PositionKeep.non;
                                forceInMove = ForceInMove.InsideMove;
                            }
                            else
                            {
                                isPositionKeep = true;
                                isForceInMove = false;
                                positionKeep = PositionKeep.Overtaking;
                                forceInMove = ForceInMove.non;
                            }
                        }
                    }
                    else 
                    {   
                        if (pList[rank + 1].runningStyle == RunningStyle.Front)
                        {
                            if (!isInsideBlocked)
                            {
                                isPositionKeep = false;
                                isForceInMove = true;
                                positionKeep = PositionKeep.non;
                                forceInMove = ForceInMove.InsideMove;
                            }
                            else
                            {
                                isPositionKeep = false;
                                isForceInMove = false;
                                positionKeep = PositionKeep.non;
                                forceInMove = ForceInMove.non;
                            }
                        }
                    }
                    break;
                case CoursePhase.Middle:
                    if (rank > 0)
                    {
                        if (isSurrounded)
                        {
                            isPositionKeep = false;
                            isForceInMove = true;
                            positionKeep = PositionKeep.non;
                            forceInMove = ForceInMove.OutsideMove;
                        }
                        else if (isBlocked && pList[rank - 1].runningStyle != RunningStyle.Runaway)
                        {
                            if (!isFrontBlocked)
                            {
                                isPositionKeep = true;
                                isForceInMove = false;
                                positionKeep = PositionKeep.Overtaking;
                                forceInMove = ForceInMove.non;
                            }
                            else if (!isInsideBlocked)
                            {
                                isPositionKeep = false;
                                isForceInMove = true;
                                positionKeep = PositionKeep.non;
                                forceInMove = ForceInMove.InsideMove;
                            }
                            else if (!isOutsideBlocked)
                            {
                                isPositionKeep = false;
                                isForceInMove = true;
                                positionKeep = PositionKeep.non;
                                forceInMove = ForceInMove.OutsideMove;
                            }
                            else
                            {
                                isPositionKeep = true;
                                isForceInMove = false;
                                positionKeep = PositionKeep.FaceDown;
                                forceInMove = ForceInMove.non;
                            }
                        }
                        else if (pList[rank - 1].runningStyle != RunningStyle.Runaway)
                        {
                            if (!isInsideBlocked)
                            {
                                isPositionKeep = false;
                                isForceInMove = true;
                                positionKeep = PositionKeep.non;
                                forceInMove = ForceInMove.InsideOvertaking;
                            }
                            else
                            {
                                isPositionKeep = true;
                                isForceInMove = false;
                                positionKeep = PositionKeep.Overtaking;
                                forceInMove = ForceInMove.non;
                            }
                        }
                        else
                        {
                            if (!isInsideBlocked)
                            {
                                isPositionKeep = false;
                                isForceInMove = true;
                                positionKeep = PositionKeep.non;
                                forceInMove = ForceInMove.InsideMove;
                            }
                            else if (pList[rank - 1].runningStyle == RunningStyle.Runaway && pList[rank - 1].currPosition.X - currPosition.X < 3)
                            {
                                isPositionKeep = true;
                                isForceInMove = false;
                                positionKeep = PositionKeep.FaceDown;
                                forceInMove = ForceInMove.non;
                            }
                            else if (pList[rank - 1].runningStyle == RunningStyle.Runaway && pList[rank - 1].currPosition.X - currPosition.X > 5)
                            {
                                isPositionKeep = true;
                                isForceInMove = false;
                                positionKeep = PositionKeep.FaceUp;
                                forceInMove = ForceInMove.non;
                            }
                            else
                            {
                                isPositionKeep = false;
                                isForceInMove = false;
                                positionKeep = PositionKeep.non;
                                forceInMove = ForceInMove.non;
                            }
                        }
                    }
                    else 
                    {
                        if (!isInsideBlocked)
                        {
                            isPositionKeep = false;
                            isForceInMove = true;
                            positionKeep = PositionKeep.non;
                            forceInMove = ForceInMove.InsideMove;
                        }
                        else if (currPosition.X - pList[rank + 1].currPosition.X > 3)
                        {
                            isPositionKeep = true;
                            isForceInMove = false;
                            positionKeep = PositionKeep.FaceDown;
                            forceInMove = ForceInMove.non;
                        }
                        else
                        {
                            isPositionKeep = false;
                            isForceInMove = false;
                            positionKeep = PositionKeep.non;
                            forceInMove = ForceInMove.non;
                        }
                    }
                    break;
                case CoursePhase.Last:
                    if (derby.IsLastStraight(currPosition.X))
                    {
                        if(isBlocked)
                        {
                            if (!isInsideBlocked)
                            {
                                isPositionKeep = false;
                                isForceInMove = true;
                                positionKeep = PositionKeep.non;
                                forceInMove = ForceInMove.InsideOvertaking;
                            }
                            else if(!isFrontBlocked)
                            {
                                isPositionKeep = true;
                                isForceInMove = false;
                                positionKeep = PositionKeep.Overtaking;
                                forceInMove = ForceInMove.non;
                            }
                            else
                            {
                                isPositionKeep = false;
                                isForceInMove = true;
                                positionKeep = PositionKeep.non;
                                forceInMove = ForceInMove.OutsideOvertaking;
                            }
                        }
                        else
                        {
                            isPositionKeep = false;
                            isForceInMove = false;
                            positionKeep = PositionKeep.non;
                            forceInMove = ForceInMove.non;
                        }
                    }
                    else
                    {
                        isPositionKeep = true;
                        isForceInMove = false;
                        positionKeep = PositionKeep.SpeedUp;
                        forceInMove = ForceInMove.non;
                    }
                    break;
                default:
                    {
                        isPositionKeep = false;
                        isForceInMove = false;
                        positionKeep = PositionKeep.non;
                        forceInMove = ForceInMove.non;
                    }
                    return;
            }
        }
        private void ActionDecideFI(List<Participant> pList)
        {
            switch (coursePhase)
            {
                case CoursePhase.First:
                    if (rank > 0)
                    {
                        if (pList[rank - 1].runningStyle == RunningStyle.Front && pList[rank - 1].currPosition.X - currPosition.X < 3)
                        {
                            isPositionKeep = true;
                            isForceInMove = false;
                            positionKeep = PositionKeep.FaceDown;
                            forceInMove = ForceInMove.non;
                        }
                        else if (pList[rank - 1].runningStyle == RunningStyle.Front && pList[rank - 1].currPosition.X - currPosition.X > 6)
                        {
                            isPositionKeep = true;
                            isForceInMove = false;
                            positionKeep = PositionKeep.FaceUp;
                            forceInMove = ForceInMove.non;
                        }
                        else if (pList[rank - 1].runningStyle == RunningStyle.Front)
                        {
                            if (!isInsideBlocked)
                            {
                                isPositionKeep = false;
                                isForceInMove = true;
                                positionKeep = PositionKeep.non;
                                forceInMove = ForceInMove.InsideMove;
                            }
                            else
                            {
                                isPositionKeep = false;
                                isForceInMove = false;
                                positionKeep = PositionKeep.non;
                                forceInMove = ForceInMove.non;
                            }
                        }
                        else if (isSurrounded)
                        {
                            isPositionKeep = false;
                            isForceInMove = false;
                            positionKeep = PositionKeep.non;
                            forceInMove = ForceInMove.non;
                        }
                        else if (isBlocked)
                        {
                            isPositionKeep = false;
                            isForceInMove = true;
                            positionKeep = PositionKeep.non;
                            forceInMove = ForceInMove.OutsideOvertaking;
                        }
                        else
                        {
                            if (isInsideBlocked)
                            {
                                isPositionKeep = false;
                                isForceInMove = true;
                                positionKeep = PositionKeep.non;
                                forceInMove = ForceInMove.InsideMove;
                            }
                            else
                            {
                                isPositionKeep = true;
                                isForceInMove = false;
                                positionKeep = PositionKeep.Overtaking;
                                forceInMove = ForceInMove.non;
                            }
                        }
                    }
                    else
                    {
                        if (pList[rank + 1].runningStyle == RunningStyle.FI)
                        {
                            if (!isInsideBlocked)
                            {
                                isPositionKeep = false;
                                isForceInMove = true;
                                positionKeep = PositionKeep.non;
                                forceInMove = ForceInMove.InsideMove;
                            }
                            else
                            {
                                isPositionKeep = false;
                                isForceInMove = false;
                                positionKeep = PositionKeep.non;
                                forceInMove = ForceInMove.non;
                            }
                        }
                    }
                    break;
                case CoursePhase.Middle:
                    if (rank > 0)
                    {
                        if (isSurrounded)
                        {
                            isPositionKeep = false;
                            isForceInMove = true;
                            positionKeep = PositionKeep.non;
                            forceInMove = ForceInMove.OutsideMove;
                        }
                        else if (isBlocked && pList[rank - 1].runningStyle != RunningStyle.Front)
                        {
                            if (!isFrontBlocked)
                            {
                                isPositionKeep = true;
                                isForceInMove = false;
                                positionKeep = PositionKeep.Overtaking;
                                forceInMove = ForceInMove.non;
                            }
                            else if (!isInsideBlocked)
                            {
                                isPositionKeep = false;
                                isForceInMove = true;
                                positionKeep = PositionKeep.non;
                                forceInMove = ForceInMove.InsideMove;
                            }
                            else if (!isOutsideBlocked)
                            {
                                isPositionKeep = false;
                                isForceInMove = true;
                                positionKeep = PositionKeep.non;
                                forceInMove = ForceInMove.OutsideMove;
                            }
                            else
                            {
                                isPositionKeep = true;
                                isForceInMove = false;
                                positionKeep = PositionKeep.FaceDown;
                                forceInMove = ForceInMove.non;
                            }
                        }
                        else if (pList[rank - 1].runningStyle != RunningStyle.Front)
                        {
                            if (!isInsideBlocked)
                            {
                                isPositionKeep = false;
                                isForceInMove = true;
                                positionKeep = PositionKeep.non;
                                forceInMove = ForceInMove.InsideOvertaking;
                            }
                            else
                            {
                                isPositionKeep = true;
                                isForceInMove = false;
                                positionKeep = PositionKeep.Overtaking;
                                forceInMove = ForceInMove.non;
                            }
                        }
                        else
                        {
                            if (!isInsideBlocked)
                            {
                                isPositionKeep = false;
                                isForceInMove = true;
                                positionKeep = PositionKeep.non;
                                forceInMove = ForceInMove.InsideMove;
                            }
                            else if (pList[rank - 1].runningStyle == RunningStyle.Front && pList[rank - 1].currPosition.X - currPosition.X < 3)
                            {
                                isPositionKeep = true;
                                isForceInMove = false;
                                positionKeep = PositionKeep.FaceDown;
                                forceInMove = ForceInMove.non;
                            }
                            else if (pList[rank - 1].runningStyle == RunningStyle.Front && pList[rank - 1].currPosition.X - currPosition.X > 5)
                            {
                                isPositionKeep = true;
                                isForceInMove = false;
                                positionKeep = PositionKeep.FaceUp;
                                forceInMove = ForceInMove.non;
                            }
                            else
                            {
                                isPositionKeep = false;
                                isForceInMove = false;
                                positionKeep = PositionKeep.non;
                                forceInMove = ForceInMove.non;
                            }
                        }
                    }
                    else
                    {
                        if (!isInsideBlocked)
                        {
                            isPositionKeep = false;
                            isForceInMove = true;
                            positionKeep = PositionKeep.non;
                            forceInMove = ForceInMove.InsideMove;
                        }
                        else if (currPosition.X - pList[rank + 1].currPosition.X > 3)
                        {
                            isPositionKeep = true;
                            isForceInMove = false;
                            positionKeep = PositionKeep.FaceDown;
                            forceInMove = ForceInMove.non;
                        }
                        else
                        {
                            isPositionKeep = false;
                            isForceInMove = false;
                            positionKeep = PositionKeep.non;
                            forceInMove = ForceInMove.non;
                        }
                    }
                    break;
                case CoursePhase.Last:
                    if (isBlocked)
                    {
                        if (!isInsideBlocked)
                        {
                            isPositionKeep = false;
                            isForceInMove = true;
                            positionKeep = PositionKeep.non;
                            forceInMove = ForceInMove.InsideOvertaking;
                        }
                        else if (!isFrontBlocked)
                        {
                            isPositionKeep = true;
                            isForceInMove = false;
                            positionKeep = PositionKeep.Overtaking;
                            forceInMove = ForceInMove.non;
                        }
                        else
                        {
                            isPositionKeep = false;
                            isForceInMove = true;
                            positionKeep = PositionKeep.non;
                            forceInMove = ForceInMove.OutsideOvertaking;
                        }
                    }
                    else
                    {
                        isPositionKeep = false;
                        isForceInMove = false;
                        positionKeep = PositionKeep.non;
                        forceInMove = ForceInMove.non;
                    }
                    break;
                default:
                    {
                        isPositionKeep = false;
                        isForceInMove = false;
                        positionKeep = PositionKeep.non;
                        forceInMove = ForceInMove.non;
                    }
                    return;
            }
        }
        private void ActionDecideStretch(List<Participant> pList)
        {
            switch (coursePhase)
            {
                case CoursePhase.First:
                    if (rank > 0)
                    {
                        if (pList[rank - 1].runningStyle == RunningStyle.FI && pList[rank - 1].currPosition.X - currPosition.X < 3)
                        {
                            isPositionKeep = true;
                            isForceInMove = false;
                            positionKeep = PositionKeep.FaceDown;
                            forceInMove = ForceInMove.non;
                        }
                        else if (pList[rank - 1].runningStyle == RunningStyle.FI && pList[rank - 1].currPosition.X - currPosition.X > 6)
                        {
                            isPositionKeep = true;
                            isForceInMove = false;
                            positionKeep = PositionKeep.FaceUp;
                            forceInMove = ForceInMove.non;
                        }
                        else if (pList[rank - 1].runningStyle == RunningStyle.FI)
                        {
                            if (!isInsideBlocked)
                            {
                                isPositionKeep = false;
                                isForceInMove = true;
                                positionKeep = PositionKeep.non;
                                forceInMove = ForceInMove.InsideMove;
                            }
                            else
                            {
                                isPositionKeep = false;
                                isForceInMove = false;
                                positionKeep = PositionKeep.non;
                                forceInMove = ForceInMove.non;
                            }
                        }
                        else if (isSurrounded)
                        {
                            isPositionKeep = false;
                            isForceInMove = false;
                            positionKeep = PositionKeep.non;
                            forceInMove = ForceInMove.non;
                        }
                        else if (isBlocked)
                        {
                            isPositionKeep = false;
                            isForceInMove = true;
                            positionKeep = PositionKeep.non;
                            forceInMove = ForceInMove.OutsideOvertaking;
                        }
                        else
                        {
                            if (isInsideBlocked)
                            {
                                isPositionKeep = false;
                                isForceInMove = true;
                                positionKeep = PositionKeep.non;
                                forceInMove = ForceInMove.InsideMove;
                            }
                            else
                            {
                                isPositionKeep = true;
                                isForceInMove = false;
                                positionKeep = PositionKeep.Overtaking;
                                forceInMove = ForceInMove.non;
                            }
                        }
                    }
                    else
                    {
                        if (pList[rank + 1].runningStyle == RunningStyle.FI)
                        {
                            if (!isInsideBlocked)
                            {
                                isPositionKeep = false;
                                isForceInMove = true;
                                positionKeep = PositionKeep.non;
                                forceInMove = ForceInMove.InsideMove;
                            }
                            else
                            {
                                isPositionKeep = false;
                                isForceInMove = false;
                                positionKeep = PositionKeep.non;
                                forceInMove = ForceInMove.non;
                            }
                        }
                    }
                    break;
                case CoursePhase.Middle:
                    if (rank > 0)
                    {
                        if (isSurrounded)
                        {
                            isPositionKeep = false;
                            isForceInMove = true;
                            positionKeep = PositionKeep.non;
                            forceInMove = ForceInMove.OutsideMove;
                        }
                        else if (isBlocked)
                        {
                            if (!isFrontBlocked)
                            {
                                isPositionKeep = true;
                                isForceInMove = false;
                                positionKeep = PositionKeep.Overtaking;
                                forceInMove = ForceInMove.non;
                            }
                            else if (!isOutsideBlocked)
                            {
                                isPositionKeep = false;
                                isForceInMove = true;
                                positionKeep = PositionKeep.non;
                                forceInMove = ForceInMove.OutsideOvertaking;
                            }
                            else
                            {
                                isPositionKeep = true;
                                isForceInMove = false;
                                positionKeep = PositionKeep.FaceDown;
                                forceInMove = ForceInMove.non;
                            }
                        }
                        else if (Math.Abs(pList[rank - 1].currPosition.Y - currPosition.Y) < 0.5)
                        {
                            isPositionKeep = false;
                            isForceInMove = true;
                            positionKeep = PositionKeep.non;
                            forceInMove = ForceInMove.OutsideMove;
                        }
                        else if(currPosition.Y > 3)
                        {
                            isPositionKeep = false;
                            isForceInMove = true;
                            positionKeep = PositionKeep.non;
                            forceInMove = ForceInMove.InsideMove;
                        }
                        else
                        {
                            isPositionKeep = true;
                            isForceInMove = false;
                            positionKeep = PositionKeep.FaceUp;
                            forceInMove = ForceInMove.non;
                        }
                    }
                    else
                    {
                        isPositionKeep = true;
                        isForceInMove = false;
                        positionKeep = PositionKeep.FaceUp;
                        forceInMove = ForceInMove.non;
                    }
                    break;
                case CoursePhase.Last:
                    if (isBlocked)
                    {
                        if (!isInsideBlocked)
                        {
                            isPositionKeep = false;
                            isForceInMove = true;
                            positionKeep = PositionKeep.non;
                            forceInMove = ForceInMove.InsideOvertaking;
                        }
                        else if (!isFrontBlocked)
                        {
                            isPositionKeep = true;
                            isForceInMove = false;
                            positionKeep = PositionKeep.Overtaking;
                            forceInMove = ForceInMove.non;
                        }
                        else
                        {
                            isPositionKeep = false;
                            isForceInMove = true;
                            positionKeep = PositionKeep.non;
                            forceInMove = ForceInMove.OutsideOvertaking;
                        }
                    }
                    else
                    {
                        isPositionKeep = false;
                        isForceInMove = false;
                        positionKeep = PositionKeep.non;
                        forceInMove = ForceInMove.non;
                    }
                    break;
                default:
                    {
                        isPositionKeep = false;
                        isForceInMove = false;
                        positionKeep = PositionKeep.non;
                        forceInMove = ForceInMove.non;
                    }
                    return;
            }
        }


        private double GetSpurtTargetSpeed()
        {
            double max = GetMaximumSpurtTargetSpeed();
            double triangle = derby.courseLength - currPosition.X;
            double square = GetRaceReferenceSpeed() - 12;
            double filledSquare = currStamina
                / (GetUmamusumeStatusCalibrateValue() * GetTurfConditionStaminaCalibrateValue(derby.turfCondition) * (1 + 200 / Math.Sqrt(600 * calibratedToughness)))
                * 144 / 20;
            if (Math.Pow(filledSquare + 2 * triangle * square, 2) - 4 * Math.Pow(2 * triangle * square, 2) < 0) return -1;
            double tempSpeed = (-filledSquare + Math.Sqrt(Math.Pow(filledSquare + 2 * triangle * square, 2) - 4 * Math.Pow(2 * triangle * square, 2)))
                / (2 * (derby.courseLength - currPosition.X));
            if (max < tempSpeed) return max;
            else return tempSpeed;
        }
        public override string ToString()
        {
            return name + " - currSpeed : " + currSpeed + " , currPosition : " + currPosition + " , maxStamina : " + maxStamina + " , currStamina : " + currStamina;
        }


    }

    public enum PositionKeep
    {
        non = 0,
        SpeedUp = 1,
        Overtaking = 2,
        FaceDown = 3,
        FaceUp = 4
    }

    public enum ForceInMove
    {
        non = 0,
        InsideOvertaking = 1,
        OutsideOvertaking = 2,
        InsideMove = 3,
        OutsideMove = 4,
        InsideCatchUp = 5,
        OutsideCatchUp = 6
    }


}
