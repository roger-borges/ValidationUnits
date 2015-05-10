using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Domain;
using System.Collections.Generic;
using ValidationUnits;
using ValidationUnits.BusinessValidationUnits;
using ValidationUnits.BusinessValidationUnits.Classes;
using ValidationUnits.ValidationModels;
using ValidationUnits.BusinessValidationUnits.SchoolCourse;

namespace Tests
{
    [TestClass]
    public class ClassTests
    {
        [TestMethod]
        public void ClassValidation_Tests()
        {
            var course = new Course()
            {
                Name = "Sci 123",
                CourseID = 1,
                Enabled = true,
                Classes = new List<Class>()
            };
            var classEmptyName = new Class() { ClassID = 1, Name = string.Empty, Course = course };
            var classValid = new Class() { ClassID = 2, Name = "Section 2", Course = course };
            var classNullCourse = new Class() { ClassID = 2, Name = "Section 2", Course = null };
            course.Classes.Add(classEmptyName);
            course.Classes.Add(classValid);
            course.Classes.Add(classNullCourse);
            //ValidationUnits.BusinessValidationUnits.DataAnnotationsValidationUnit
           
            var validationResult1 = Validator.Validate(classEmptyName,
                    ValidatorSettings.CreateDefaultValidatorSettings(),
                    new ClassNameRequiredValidationUnit(),
                    new ClassHasCourseValidationUnit());
            Assert.AreEqual(true, validationResult1.HasErrors);
            Assert.AreEqual(1, validationResult1.ErrorMessages.Count);

            var validationResult2 = Validator.Validate(classValid,
                    ValidatorSettings.CreateDefaultValidatorSettings(),
                    new ClassNameRequiredValidationUnit(),
                    new ClassHasCourseValidationUnit());
            Assert.AreEqual(false, validationResult2.HasErrors);

            var validationResult3 = Validator.Validate(classNullCourse,
                    ValidatorSettings.CreateDefaultValidatorSettings(),
                    new ClassNameRequiredValidationUnit(),
                    new ClassHasCourseValidationUnit());
            Assert.AreEqual(true, validationResult3.HasErrors);
            Assert.AreEqual(1, validationResult3.ErrorMessages.Count);

        }

        [TestMethod]
        public void SchoolCourseValidation_Tests()
        {
            var course = new Course()
            {
                Name = "Sci 123",
                CourseID = 1,
                Enabled = true,
                Classes = new List<Class>()
            };
            var classEmptyName = new Class() { ClassID = 1, Name = string.Empty, Course = course };
            var classValid = new Class() { ClassID = 2, Name = "Section 2", Course = course };
            var classNullCourse = new Class() { ClassID = 2, Name = "Section 2", Course = null };
            course.Classes.Add(classEmptyName);
            course.Classes.Add(classValid);
            course.Classes.Add(classNullCourse);

            var schoolCourseModel = new SchoolCourseValidationModel()
            {
                 Course = course,
                 Classes = course.Classes.ToArray()
            };
                
            var testResult = Validator.Validate(schoolCourseModel,
                    ValidatorSettings.CreateDefaultValidatorSettings(), 
                    new SchoolCourseAssociationValidationUnit());

            Assert.AreEqual(true, testResult);
        }
    }
}
