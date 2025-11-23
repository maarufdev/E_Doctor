document.addEventListener("DOMContentLoaded", () => {
    $("#closeAlertModal").off("click").on("click", function () {
        $("#registrationAlert").removeClass("visible");
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