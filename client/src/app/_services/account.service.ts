import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { User } from '../_models/user';
import { Observable, ReplaySubject } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})

export class AccountService{

  public currentUserSource = new ReplaySubject<User | null>(1);



  baseUrl = environment.apiUrl;

  //isLogin: boolean = false;

  //Create an observable to store the user in

  //currentUserSource - private property and we gonna set it to type of observable -ReplaySubject
  //ReplaySubject - store the value evry time we subscribe to this observable his gonna emmit the last value or the many values we want to emmit
  //The number determen how meny values (versions) we gonna store, or it going to be null or the current object





  //By an convection, because this is observable object we sign it with a $ sign at the and
  currentUser$ = this.currentUserSource.asObservable();


  constructor(private http: HttpClient) { }

  register(model: User){
    return this.http.post<User>(this.baseUrl + 'account/register', model).pipe(
      map((user) => {
        if (user) {
          this.setCurrentUser(user);
        }
        return user;
      })
    );
  }

  login(model:any){
    return this.http.post<User>(this.baseUrl + 'account/login', model).pipe(
      map((response) => {
        const user = response;
        if(user){
          this.setCurrentUser(user);
        }
      })
    );
  }

  setCurrentUser(user: User){
    user.roles = [];
    const roles = this.getDecodedToken(user.token).role;
    Array.isArray(roles) ? user.roles = roles : user.roles.push(roles);
    localStorage.setItem('user', JSON.stringify(user));
    this.currentUserSource.next(user); //this is how we store the next value in obsevable object
  }


  logout(){
    localStorage.removeItem('user');
    this.currentUserSource.next(null);
  }


  getDecodedToken(token){
    return JSON.parse(atob(token.split('.')[1]));
  }




}
