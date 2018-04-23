using System;
using DevExpress.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Data.Filtering;

namespace WinSolution.Module {
    [DefaultClassOptions]
    [System.ComponentModel.DefaultProperty("Title")]
    public class Department : BaseObject {
        private string title;
        private string office;
        public Department(Session session) : base(session) { }
        public string Title {
            get { return title; }
            set {
                SetPropertyValue("Title", ref title, value);
            }
        }
        public string Office {
            get { return office; }
            set {
                SetPropertyValue("Office", ref office, value);
            }
        }
        [Association("Department-Employees"), Aggregated]
        public XPCollection<EmployeeBase> Employees {
            get { return GetCollection<EmployeeBase>("Employees"); }
        }
        private XPCollection<LocalEmployee> _LocalEmployees;
        [Custom("AllowEdit", "False")]
        public XPCollection<LocalEmployee> LocalEmployees {
            get {
                if (_LocalEmployees == null)
                    _LocalEmployees = new XPCollection<LocalEmployee>(Session,
                        new GroupOperator(
                        new BinaryOperator(BaseObject.Fields.ObjectType.TypeName, new OperandValue(typeof(LocalEmployee).FullName), BinaryOperatorType.Equal),
                        new BinaryOperator("Department", this)));
                return _LocalEmployees;
            }
        }
        private XPCollection<ForeignEmployee> _ForeignEmployees;
        [Custom("AllowEdit", "False")]
        public XPCollection<ForeignEmployee> ForeignEmployees {
            get {
                if (_ForeignEmployees == null)
                    _ForeignEmployees = new XPCollection<ForeignEmployee>(Session,
                        new GroupOperator(
                        new BinaryOperator(BaseObject.Fields.ObjectType.TypeName, new OperandValue(typeof(ForeignEmployee).FullName), BinaryOperatorType.Equal),
                        new BinaryOperator("Department", this)));
                return _ForeignEmployees;
            }
        }
        public void UpdateLocalEmployees() {
            LocalEmployees.HintCollection = Employees;
            OnChanged("LocalEmployees");
        }
        public void UpdateForeignEmployees() {
            ForeignEmployees.HintCollection = Employees;
            OnChanged("ForeignEmployees");
        }
        protected override void OnLoaded() {
            base.OnLoaded();
            Reset();
        }
        private void Reset() {
            _LocalEmployees = null;
            _ForeignEmployees = null;
        }
    }
    public abstract class EmployeeBase : BaseObject {
        public EmployeeBase(Session session) : base(session) { }
        private string name;
        private string email;
        public string Name {
            get { return name; }
            set {
                SetPropertyValue("Name", ref name, value);
            }
        }
        public string Email {
            get { return email; }
            set {
                SetPropertyValue("Email", ref email, value);
            }
        }
        private Department department;
        [Association("Department-Employees")]
        public Department Department {
            get { return department; }
            set {
                Department oldDepartment = department;
                SetPropertyValue("Department", ref department, value);
                if (!IsLoading && !IsSaving && oldDepartment != department) {
                    oldDepartment = oldDepartment ?? department;
                    oldDepartment.UpdateLocalEmployees();
                    oldDepartment.UpdateForeignEmployees();
                }
            }
        }
    }
    public class LocalEmployee : EmployeeBase {
        public LocalEmployee(Session session) : base(session) { }
        private string insurancePolicyNumber;
        public string InsurancePolicyNumber {
            get { return insurancePolicyNumber; }
            set {
                SetPropertyValue("InsurancePolicyNumber", ref insurancePolicyNumber, value);
            }
        }
    }
    public class ForeignEmployee : EmployeeBase {
        public ForeignEmployee(Session session) : base(session) { }
        private DateTime visaExpirationDate;
        public DateTime VisaExpirationDate {
            get { return visaExpirationDate; }
            set {
                SetPropertyValue("VisaExpirationDate", ref visaExpirationDate, value);
            }
        }
    }

}