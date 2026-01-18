document.addEventListener("DOMContentLoaded", () => {
    $("#closeAlertModal").off("click").on("click", function () {
        $("#registrationAlert").removeClass("visible");
    });

    const prevHospitalizationElmts = {
        prevHospitalizationCkbx: $(`#past-PrevHospitalization`),
        previousHosptlTxt: $(`#PastMedicalRecord_PreviousHospitalizationText`),
    };

    prevHospitalizationElmts.previousHosptlTxt.prop(
        "disabled",
        !prevHospitalizationElmts.prevHospitalizationCkbx.prop("checked"))

    prevHospitalizationElmts.prevHospitalizationCkbx.off("change").on("change", function (e) {
        const $this = $(this);
        const isChecked = $this.prop("checked");

        if (isChecked) {
            prevHospitalizationElmts.previousHosptlTxt.removeAttr("disabled");
            prevHospitalizationElmts.previousHosptlTxt.trigger("focus");
        } else {
            prevHospitalizationElmts.previousHosptlTxt.attr("disabled");
            prevHospitalizationElmts.previousHosptlTxt.val("");
        }
    });


    // setup allergyToMeds
    const prevHostAllergyToMedsElmts = {
        pastAllergyToMedsCkbx: $(`#past-AllergyToMeds`),
        pastAllergyToMedsTxt: $(`#PastMedicalRecord_MedAllergyText`),
    }

    prevHostAllergyToMedsElmts.pastAllergyToMedsTxt.prop(
        "disabled",
        !prevHostAllergyToMedsElmts.pastAllergyToMedsCkbx.prop("checked")
    );

    prevHostAllergyToMedsElmts.pastAllergyToMedsCkbx.off("change").on("change", function (e) {
        const $this = $(this);
        const isChecked = $this.prop("checked");

        if (isChecked) {
            prevHostAllergyToMedsElmts.pastAllergyToMedsTxt.removeAttr("disabled");
            prevHostAllergyToMedsElmts.pastAllergyToMedsTxt.trigger("focus");
        } else {
            prevHostAllergyToMedsElmts.pastAllergyToMedsTxt.attr("disabled");
            prevHostAllergyToMedsElmts.pastAllergyToMedsTxt.val("");
        }

    });

    const famHistFields = [...$(`[name^="FamilyHistory."]:not(#fh-None)`)];

    $("#fh-None").off("change").on("change", function (e) {
        const $this = $(this);
        const isChecked = $this.prop("checked");

        famHistFields.forEach(item => {
            $(item).prop("disabled", isChecked);

            if (isChecked) {
                $(item).prop("checked", false);
            }

            if ($(item).attr("type") == "text") {
                $(item).val("");
            }
        });
    })

    famHistFields.forEach(item => $(item).prop("disabled", true));
});