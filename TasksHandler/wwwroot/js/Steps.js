function AddStepHandle() {
    TaskEditVM.steps.push(new StepViewModel({ editMode: true, done: false }));
    $("[name=txtStepDescription]:visible").focus();
}