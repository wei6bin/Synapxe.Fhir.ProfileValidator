# FHIR Validation

## Start
FHIR validation is the process of checking whether a FHIR resource conforms to the rules and constraints defined in the FHIR specification, and the associated profiles. 

The validation ensures the data quality, consistent, accurate and complete. Here I will demonstrate the **native FHIR validation** and the profile validation by customize the **Structure Definition**. 

The showcase is based on the Appoinment resource.

## Resource Structure Validation
FHIR resources have a defined structure, specifying the allowed elements, data types, and relationships, it provides the native validation mechanisms to ensure the resource to conform to the defined specifications.

e.g. it does not accept the un-defined element from payload. - as 'date' is not a defined element.

e.g. the element 'participant' is defined as array but pass in as single value

## Cardinality Validation
FHIR specifies minimum and maximum cardinality for each element within a resource. 

For the System will throw error when the mandatory field 'status' is missing.

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

[FHIRPath](https://hl7.org/fhirpath/)

[FHIRPath Demo Page](https://hl7.github.io/fhirpath.js/)

# Hands On (Windows WSL Ubuntu)
0. Make sure https://nexus.ihis-hip.sg/repository/ihis-nuget/ in the nuget package list.
```
dotnet nuget list source

Registered Sources:
  1.  nuget.org [Enabled]
      https://api.nuget.org/v3/index.json
  2.  nexus [Enabled]
      https://nexus.ihis-hip.sg/repository/ihis-nuget/
```
1. Make sure FhirEngine templated installed
```
dotnet new --list | grep Fhir

FhirEngine Web Api                            fhirengine-webapi  [C#]
```
2. Create project based on FHIRNexus template, framework net6.0 and db store type is Document DB.

```
mkdir Synapxe.Fhir.Handson1
cd Synapxe.Fhir.Handson1
dotnet new sln
dotnet new fhirengine-webapi --includetest=false --framework=net6.0 --dbstore=Document
```
3. Add 'Patient' resource at capability-statement.json

_hint: refer to 'Appointment' section. Keep all interaction and leave searchParam and Operation as empty as following:_

"searchParam": []

"operation": []

4. New StructureDefinition file **Patient-custom.StructureDefinition.json** under Conformance folder

_hint: refer to the sample [file](https://github.com/wei6bin/Synapxe.Fhir.ProfileValidator/blob/main/Synapxe.Fhir.ProfileValidator/Conformance/Patient-custom.StructureDefinition.json)_

5. Modify the appsettings.json, to allow accept 'Patint' resource type at FhirDataStore (line 29)

6. Create http request, refer to [patient.http](https://github.com/wei6bin/Synapxe.Fhir.ProfileValidator/blob/main/Synapxe.Fhir.ProfileValidator/Sample%20Requests/patient.http) from sample project

7. Refer to https://github.com/bruce4000www/Synapxe.FhirNexus.ExtendedOperation/blob/master/README.md (from step 4 onwards) for how to add extended operation of $get-patient-by-nric