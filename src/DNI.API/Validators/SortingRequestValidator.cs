using System.Linq;

using DNI.API.Requests;

using FluentValidation;

namespace DNI.API.Validators {
    /// <summary>
    ///     Validates a <see cref="GetShowsRequest" /> for the API
    /// </summary>
    public class GetShowsRequestValidator : AbstractValidator<GetShowsRequest> {
        private readonly string[] acceptedFieldValues = {"PublishedTime", "Title", "DurationInSeconds"};

        public GetShowsRequestValidator() {
            var acceptedFieldsString = string.Concat("'", string.Join("', '", acceptedFieldValues), "'");
            RuleFor(x => x.Field)
                .NotNull()
                .NotEmpty()
                .WithMessage("Field is required")
                .Must(x => acceptedFieldValues.Contains(x))
                .WithMessage($"The specified 'sort-field' is not valid. Possible values: {acceptedFieldsString}.");
        }
    }
}