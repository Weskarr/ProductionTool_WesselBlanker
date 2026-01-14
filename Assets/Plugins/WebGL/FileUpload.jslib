mergeInto(LibraryManager.library, {
  UploadFile: function () {
    const input = document.createElement("input");
    input.type = "file";
    input.accept = ".json,application/json";

    input.onchange = e => {
      const file = e.target.files[0];
      if (!file) return;

      const reader = new FileReader();

      reader.onload = () => {
        SendMessage(
          "SaveDirectLoader",
          "OnFileLoaded",
          reader.result
        );
      };

      reader.readAsText(file);
    };

    input.click();
  }
});
