using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Veterinario_MVVM.ViewModels.Base
{
    public abstract class ValidatableViewModel : BaseViewModel, INotifyDataErrorInfo
    {
        private readonly Dictionary<string, List<string>> _errors = new();
        public bool HasErrors => _errors.Values.Any(l => l.Count > 0);
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public IEnumerable GetErrors(string? prop) => string.IsNullOrEmpty(prop)
            ? _errors.Values.SelectMany(x => x).ToList()
            : _errors.TryGetValue(prop, out var e) ? e : Enumerable.Empty<string>();

        protected void OnErrorsChanged([CallerMemberName] string? prop = null)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(prop));
            OnPropertyChanged(nameof(HasErrors));
            CommandManager.InvalidateRequerySuggested();
        }

        public void AddError(string prop, string err)
        {
            if (!_errors.ContainsKey(prop)) _errors[prop] = new List<string>();
            if (!_errors[prop].Contains(err)) { _errors[prop].Add(err); OnErrorsChanged(prop); }
        }

        public void ClearErrors([CallerMemberName] string? prop = null)
        {
            if (string.IsNullOrEmpty(prop))
            {
                // Avisamos a cada propiedad que tenía errores para que la vista quite los adornos
                List<string> propiedades = _errors.Keys.ToList();
                _errors.Clear();
                foreach (string propiedad in propiedades) OnErrorsChanged(propiedad);
                OnErrorsChanged(string.Empty);
                return;
            }
            if (_errors.Remove(prop)) OnErrorsChanged(prop);
        }

        public void SetErrors(string prop, IEnumerable<string> errs)
        {
            ClearErrors(prop);
            var list = errs.ToList();
            if (list.Count > 0) { _errors[prop] = list; OnErrorsChanged(prop); }
        }

        protected void ValidateProperty(object? val, [CallerMemberName] string? prop = null)
        {
            if (string.IsNullOrEmpty(prop)) return;
            var res = new List<ValidationResult>();
            Validator.TryValidateProperty(val, new ValidationContext(this) { MemberName = prop }, res);
            if (res.Count > 0) SetErrors(prop, res.Select(r => r.ErrorMessage ?? "Error"));
            else ClearErrors(prop);
        }

        public bool ValidateAll()
        {
            var res = new List<ValidationResult>();
            List<string> anteriores = _errors.Keys.ToList();
            _errors.Clear();
            Validator.TryValidateObject(this, new ValidationContext(this), res, validateAllProperties: true);
            foreach (var g in res.GroupBy(r => r.MemberNames.FirstOrDefault() ?? string.Empty))
                if (!string.IsNullOrEmpty(g.Key)) _errors[g.Key] = g.Select(r => r.ErrorMessage ?? "Error").ToList();

            // Notificamos a la vista cada propiedad que ha cambiado de estado (como en ValidateAll de las diapositivas)
            foreach (string propiedad in anteriores.Union(_errors.Keys))
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propiedad));

            OnPropertyChanged(nameof(HasErrors));
            CommandManager.InvalidateRequerySuggested();
            return !HasErrors;
        }
    }
}
