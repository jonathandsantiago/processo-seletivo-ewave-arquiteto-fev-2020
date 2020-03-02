import {Component, OnInit} from '@angular/core';
import {Router, ActivatedRoute} from '@angular/router';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {finalize, first} from 'rxjs/operators';
import {LoginService} from "./login.service";
import {MatSnackBar} from "@angular/material/snack-bar";

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  loginForm: FormGroup;
  submitted = false;
  returnUrl: string;
  error = '';

  constructor(
    private formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    protected snackbar: MatSnackBar,
    private loginService: LoginService
  ) {
    if (this.loginService.currentUser) {
      this.router.navigate(['/']);
    }
  }

  ngOnInit() {
    this.loginForm = this.formBuilder.group({
      login: ['', Validators.required],
      password: ['', Validators.required]
    });

    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
  }

  get form() {
    return this.loginForm.controls;
  }

  onSubmit() {
    this.submitted = true;

    if (this.loginForm.invalid) {
      return;
    }
    const loading = this.snackbar.open('Logando aguarde...');

    this.loginService.login(this.form.login.value, this.form.password.value)
      .pipe(first(), finalize(() => {
        loading.dismiss();
      }))
      .subscribe(
        data => {
          this.router.navigate([this.returnUrl]);
        },
        (error: Error) => {
          this.snackbar.open(error.message, null, {duration: 2000})
        });
  }
}
