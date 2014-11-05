using System.ComponentModel.DataAnnotations;
using SbManager.Models;
using NUnit.Framework;
using Shouldly;
using TestStack.BDDfy;
using ValidationResult = SbManager.Models.ValidationResult;

namespace SbManager.Tests.Models
{
    [TestFixture]
    public class ModelExtensionsTests
    {
        private ValidationResult _result;
        private TestWithValidation _model;

        [Test]
        public void CanValidateCorrectModel()
        {
            this.Given(x => GivenAValidModel())
                .When(x => x.WhenValidating())
                .Then(x => x.ThenIsValidShouldBe(true))
                .Then(x => x.ThenShouldHaveNErrors(0))
                .BDDfy();
        }

        [Test]
        public void CanGetErrorsForInvalidModel()
        {
            this.Given(x => GivenAnInvalidModel())
                .When(x => x.WhenValidating())
                .Then(x => x.ThenIsValidShouldBe(false))
                .Then(x => x.ThenShouldHaveNErrors(2))
                .BDDfy();
        }

        void GivenAValidModel()
        {
            _model = new TestWithValidation
            {
                Foo = "a",
                Bar = "b",
                Baz = "c",
            };
        }
        void GivenAnInvalidModel()
        {
            _model = new TestWithValidation
            {
                Baz = "c",
            };
        }

        void WhenValidating()
        {
            _result = _model.Validate();
        }

        void ThenIsValidShouldBe(bool isvalid)
        {
            _result.IsValid.ShouldBe(isvalid);
        }
        void ThenShouldHaveNErrors(int numErrors)
        {
            if (numErrors == 0) _result.Exception.ShouldBe(null);
            else _result.Exception.InnerExceptions.Count.ShouldBe(numErrors);
        }
    }

    public class TestWithValidation
    {
        [Required]
        public string Foo { get; set; }
        [Required]
        public string Bar { get; set; }
        [Required]
        public string Baz { get; set; }
    }
}
