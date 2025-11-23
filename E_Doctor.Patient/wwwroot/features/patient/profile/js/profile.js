document.addEventListener("DOMContentLoaded", () => {
    $("#closeAlertModal").off("click").on("click", function () {
        $("#registrationAlert").removeClass("visible");
    });

    const famHistFields = [...$(`[name^="FamilyHistory."]:not(#fh-None)`)];

    $("#fh-None").off("change").on("change", function (e) {
        const $this = $(this);
        const isNoneChecked = $this.prop("checked");

        famHistFields.forEach(item => {
            $(item).prop("disabled", isNoneChecked);

            if (isNoneChecked) {
                $(item).prop("checked", false);
            }

            if ($(item).attr("type") == "text") {
                $(item).val("");
            }
        });
    })

    $("#saveChangesButton").off("click").on("click", function () {
        // Manually trigger the form submission when the button is clicked
        // This will post the form data to the server as defined by asp-action.
        $("#register-form").submit();
    });

    famHistFields.forEach(item => $(item).prop("disabled", true));
});