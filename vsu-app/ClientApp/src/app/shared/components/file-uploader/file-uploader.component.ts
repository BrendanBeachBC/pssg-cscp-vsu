import { Component, ViewChild, ElementRef, Input } from '@angular/core';
import { FormBuilder, FormArray } from '@angular/forms';
import { config } from '../../../../config';

@Component({
  selector: 'app-file-uploader',
  templateUrl: './file-uploader.component.html',
  styleUrls: ['./file-uploader.component.scss']
})
export class FileUploaderComponent {
  @ViewChild('files', {static: true}) myInputVariable: ElementRef;
  @Input() formType: number;
  @Input() documents: FormArray;


  constructor(private fb: FormBuilder) { }

  fakeBrowseClick(): void {
    this.myInputVariable.nativeElement.value = "";
    // the UI element for the native element style doesn't match so we hide it and fake the user click.
    this.myInputVariable.nativeElement.click();
  }

  onFilesAdded(files: FileList): void {
    for (let i = 0; i < files.length; i++) {
      // convert the file to base64 for upload
      const reader: FileReader = new FileReader();
      reader.readAsDataURL(files.item(i));
      reader.onload = () => {
        let body = reader.result.toString();
        body = body.split(',').slice(-1)[0];
        let fileIndex = this.documents.controls.findIndex(doc => doc.get('filename').value === files.item(i).name);
        if (fileIndex >= 0) {
          this.documents.controls[fileIndex].get('body').patchValue(body);
        }
        else {
          let file_extenstion = files.item(i).name.trim().split('.').pop();
          if (config.accepted_file_extensions[file_extenstion]) {
            this.documents.push(this.fb.group({
              filename: [files.item(i).name],
              body: [body],
              subject: ['']
            }));
          }
        }
      };
      reader.onerror = error => console.log('Error: ', error);
    }
  }
  removeItem(index: number): void {
    this.documents.removeAt(index);
  }
}