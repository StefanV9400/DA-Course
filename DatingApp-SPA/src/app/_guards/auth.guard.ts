import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/Alertify.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate  {

  constructor(private authService: AuthService, private router: Router,
              private alertify: AlertifyService) {}

  canActivate(): Observable<boolean> | Promise<boolean> | boolean{
    if (this.authService.loggedIn()) {
      return true;
    }

    this.alertify.error('You are not logged in.');
    this.router.navigate(['/home']);
    return false;
    
  }
}
