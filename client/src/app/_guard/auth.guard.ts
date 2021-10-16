import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(private accountService: AccountService, private toastr: ToastrService) {
  }

  canActivate(): Observable<boolean> | boolean{
    return this.accountService.currentUsers$.pipe(
      map((user:any) => {
        if(user) return true;
        // console.log('not pass');
        this.toastr.error('You shall not pass!');
        return false;
      })
    )
  }
}
