# FHIR Validation

## Start
Validating FHIR Resource is an important aspect of working with FHIR.
Here I will demonstrate the **native FHIR validation** and the profile validation by customize the **Structure Definition**. Below showcase based on the Appoinment resource.

## Resource Structure Validation
FHIR resources have a defined structure, specifying the allowed elements, data types, and relationships, it provides the native validation mechanisms to ensure the resource to conform to the defined specifications.

e.g. it does not accept the un-defined element from payload.
e.g. the element 'participant' is defined as array but pass in as single value

## Cardinality Validation
FHIR specifies minimum and maximum cardinality for each element within a resource. System will throw error when the mandatory field 'status' is missing.

We can use StructureDefinition to define a profile that constrains or extends the base definition of a resource.
The optional data field, the cardinality is 0..1 or 0..*, we can change it to mandatory by modify the minimal to 1 ("min": 1), so that the system expects it as mandatory field.

Another case is to exclude the element by setting the maximum to 0, so that system will reject any request that includes the 'created' field, treating it as if were not supported.

## Data Type Validation
FHIR defines specific data types for each element (e.g., string, code, integer), it detects the error when the data type mismatchs.
The case here the 'priority' is a code type, it fails the validation when pass in the string type.

For some data type, it self defines the data range, for example the minutesDuration of the Appointment, the data type is positiveInt, which not allow the negative value pass over.

It supports the min and max setting of the field, minValuePositiveInt and maxValuePositiveInt

## Code System Validation
For the a code used in an element, native validation checks that the code is a valid member of that code system.
e.g. system identified an error when the field 'status' value is 'completed', which does not follow the pre-defined code by FHIR.

Another code system is the User defined ValueSet.
FHIR allows specifying ValueSet to restrict the possible values for certain elements. Native validation ensures that values comply with the defined value sets.
From the Appointment-custom.StructureDefinition.json, the element cancellationReason is binded to a predefined valueset, refer to the valueset list at valueset.json. 

## Reference Validation
For elements that represent references to other resources, native validation checks that the reference is valid, points to an existing resource, and is of the correct type.
In this example, system to perform patient resource id check at Patient resource when creating a Appointment resource, it rejects the request when patient resource id is not found.

There is plugin setting needs to turn on for the reference check feature.

## FHIR resource native rule validation
There are native rules defined at each resources.

The [Appointment](https://www.hl7.org/fhir/appointment.html) resource, "+ Rule: The start must be less than or equal to the end", which done by SDK.

## FHIRPath Expression Evaluation (Custom Validation)
We can add more customize validation logic on the data range, the [FHIRPath](https://build.fhir.org/fhirpath.html) is like the XPath, operations are expressed in terms of logical content of data models, supporting selection and filtering of data. 
Example here, the appointment start and end SHALL not be past date, custom rule setting to validate the date range. 

## Extensions Validation
[Extension](http://hl7.org/fhir/extensibility.html) - used for adding addtional custom elements or data to resources beyond what's defined in the base specification. 
We might need user to submit a condition tag info when requesting an appointment, the condition-tag is pre-defined ValueSet, user shall follow the data definition submit the codeableConcept, instead of a string text.


## Extensions Validation - Slicing
Slicing is a mechanism in FHIR that allows you to create subsets or groups of repeating elements within a resource. 

In the case of extensions, slicing can be used to organize multiple extensions under a common structure.

Example, we defined 2 extensions for Appointment resource, the value of condition-tag is codeableConcept while the value of mobile-number is a string. By define the slice to separate the extensions to 2 different types.

Looking at the 1st extension condition-tag, the cardinality defines the maximum is 2 and ValueSet from hsg-condition-tag.
For the 2nd extension mobile-number, the cardinality is 0..1, and furthermore we can use the FHIR [Expression](http://hl7.org/fhirpath/#expressions) to restrict the data pattern to follow the Singapore mobile number.

# Appendix
[FHIR Resource List](https://www.hl7.org/fhir/resourcelist.html)

[FHIR Profile](https://build.fhir.org/profiling.html)

[FHIR Path](https://hl7.org/fhir/fhirpath.html)